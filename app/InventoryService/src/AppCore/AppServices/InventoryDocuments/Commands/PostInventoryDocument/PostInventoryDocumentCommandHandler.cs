namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.PostInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;

public class PostInventoryDocumentCommandHandler
    : CommandHandler<PostInventoryDocumentCommand, PostInventoryDocumentCommandResult>
{
    private readonly IInventoryDocumentCommandRepository _documentRepository;
    private readonly IInventoryTransactionCommandRepository _transactionRepository;
    private readonly IStockDetailCommandRepository _stockDetailRepository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;

    public PostInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventoryTransactionCommandRepository transactionRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository)
    {
        _documentRepository = documentRepository;
        _transactionRepository = transactionRepository;
        _stockDetailRepository = stockDetailRepository;
        _sourceBalanceRepository = sourceBalanceRepository;
    }

    public override async Task<CommandResult<PostInventoryDocumentCommandResult>> Handle(PostInventoryDocumentCommand command)
    {
        var document = await _documentRepository.GetGraphAsync(BusinessKey.FromGuid(command.DocumentBusinessKey));
        if (document is null || document.Id == 0)
            return Fail("Inventory document was not found.");

        if (document.Status == InventoryDocumentStatus.Posted)
            return Fail("Inventory document is already posted.");

        var transaction = InventoryTransaction.Create(
            transactionNo: $"TX-{document.DocumentNo}",
            transactionType: MapTransactionType(document.DocumentType),
            warehouseRef: document.WarehouseRef,
            sellerRef: document.SellerRef,
            occurredAt: document.OccurredAt,
            referenceType: nameof(InventoryDocument),
            referenceBusinessId: document.BusinessKey.Value,
            correlationId: document.CorrelationId,
            idempotencyKey: document.IdempotencyKey,
            reasonCode: document.ReasonCode);

        var effects = BuildEffects(document).ToList();
        foreach (var effect in effects)
        {
            var line = effect.TransactionLine;
            transaction.AddLine(line);

            var locationRef = line.BaseQtyDelta >= 0
                ? line.DestinationLocationRef
                : line.SourceLocationRef;

            var qualityStatusRef = line.BaseQtyDelta >= 0
                ? line.NewQualityStatusRef
                : line.OldQualityStatusRef;

            if (locationRef is null || qualityStatusRef is null)
                return Fail("Posting requires resolved location and quality status for every stock effect.");

            var stockDetail = await _stockDetailRepository.FindByBucketAsync(
                line.VariantRef,
                document.SellerRef,
                document.WarehouseRef,
                locationRef.Value,
                qualityStatusRef.Value,
                line.LotBatchNo);

            if (stockDetail is null)
            {
                if (line.BaseQtyDelta < 0)
                    return Fail("Cannot issue or reduce stock from a bucket that does not exist.");

                stockDetail = StockDetail.Create(
                    line.VariantRef,
                    document.SellerRef,
                    document.WarehouseRef,
                    locationRef.Value,
                    qualityStatusRef.Value,
                    line.LotBatchNo,
                    0,
                    document.OccurredAt);

                await _stockDetailRepository.InsertAsync(stockDetail);
            }

            stockDetail.ApplyQuantity(line.BaseQtyDelta, document.OccurredAt);
            line.LinkStockDetail(stockDetail.BusinessKey);
        }

        var sourceTracingResult = await ApplySourceTracingAsync(document, transaction, effects);
        if (!sourceTracingResult.Success)
            return Fail(sourceTracingResult.Error ?? "Source tracing failed.");

        transaction.MarkPosted();
        await _transactionRepository.InsertAsync(transaction);
        document.MarkPosted(transaction.BusinessKey, command.PostedBy);
        await _transactionRepository.CommitAsync();

        return Ok(new PostInventoryDocumentCommandResult
        {
            DocumentBusinessKey = document.BusinessKey.Value,
            TransactionBusinessKey = transaction.BusinessKey.Value,
            Status = document.Status.ToString()
        });
    }

    private static InventoryTransactionType MapTransactionType(InventoryDocumentType documentType)
    {
        return documentType switch
        {
            InventoryDocumentType.Receipt => InventoryTransactionType.Receipt,
            InventoryDocumentType.Issue => InventoryTransactionType.Issue,
            InventoryDocumentType.Transfer => InventoryTransactionType.Transfer,
            InventoryDocumentType.Adjustment => InventoryTransactionType.Adjustment,
            InventoryDocumentType.Return => InventoryTransactionType.Return,
            InventoryDocumentType.QualityChange => InventoryTransactionType.QualityChange,
            _ => InventoryTransactionType.Adjustment
        };
    }

    private async Task<(bool Success, string? Error)> ApplySourceTracingAsync(
        InventoryDocument document,
        InventoryTransaction transaction,
        IReadOnlyList<InventoryDocumentPostingEffect> effects)
    {
        foreach (var effect in effects)
        {
            var line = effect.TransactionLine;
            if (line.BaseQtyDelta > 0)
            {
                var sourceType = ResolveSourceType(document.DocumentType, line.BaseQtyDelta);
                if (sourceType is null)
                    continue;

                var locationRef = line.DestinationLocationRef;
                var qualityStatusRef = line.NewQualityStatusRef;
                if (locationRef is null || qualityStatusRef is null)
                    return (false, "Opening a source balance requires destination location and quality status.");

                var sourceBalance = InventorySourceBalance.Open(
                    sourceType.Value,
                    line.VariantRef,
                    document.SellerRef,
                    document.WarehouseRef,
                    locationRef.Value,
                    qualityStatusRef.Value,
                    line.BaseUomRef,
                    line.BaseQtyDelta,
                    line.LotBatchNo,
                    document.BusinessKey.Value,
                    effect.DocumentLineBusinessKey,
                    transaction.BusinessKey.Value,
                    line.BusinessKey.Value,
                    line.SerialRef);

                await _sourceBalanceRepository.InsertAsync(sourceBalance);
                continue;
            }

            if (line.BaseQtyDelta < 0)
            {
                if (!ShouldConsumeSource(document.DocumentType))
                    continue;

                var locationRef = line.SourceLocationRef;
                var qualityStatusRef = line.OldQualityStatusRef;
                if (locationRef is null || qualityStatusRef is null)
                    return (false, "Consuming source balance requires source location and quality status.");

                var remainingToConsume = Math.Abs(line.BaseQtyDelta);
                var sourceBalances = await _sourceBalanceRepository.GetOpenByBucketAsync(
                    line.VariantRef,
                    document.SellerRef,
                    document.WarehouseRef,
                    locationRef.Value,
                    qualityStatusRef.Value,
                    line.LotBatchNo,
                    line.SerialRef);

                foreach (var sourceBalance in sourceBalances)
                {
                    if (remainingToConsume <= 0)
                        break;

                    var consumedQty = Math.Min(sourceBalance.AvailableQty, remainingToConsume);
                    if (consumedQty <= 0)
                        continue;

                    sourceBalance.Consume(
                        consumedQty,
                        transaction.BusinessKey.Value,
                        line.BusinessKey.Value,
                        line.ReasonCode ?? document.ReasonCode);

                    remainingToConsume -= consumedQty;
                }

                if (remainingToConsume > 0)
                {
                    return (false, "Open source balances are not enough to cover this outbound inventory document.");
                }
            }
        }

        return (true, null);
    }

    private static InventorySourceType? ResolveSourceType(InventoryDocumentType documentType, decimal baseQtyDelta)
    {
        if (baseQtyDelta <= 0)
            return null;

        return documentType switch
        {
            InventoryDocumentType.Receipt => InventorySourceType.Receipt,
            InventoryDocumentType.Return => InventorySourceType.ReturnRestock,
            InventoryDocumentType.Adjustment => InventorySourceType.AdjustmentIncrease,
            _ => null
        };
    }

    private static bool ShouldConsumeSource(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue or InventoryDocumentType.Adjustment;

    private static IEnumerable<InventoryDocumentPostingEffect> BuildEffects(InventoryDocument document)
    {
        foreach (var line in document.Lines)
        {
            switch (document.DocumentType)
            {
                case InventoryDocumentType.Receipt:
                case InventoryDocumentType.Return:
                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            line.BaseQty,
                            line.BaseUomRef,
                            destinationLocationRef: line.DestinationLocationRef,
                            newQualityStatusRef: line.QualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));
                    break;

                case InventoryDocumentType.Issue:
                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            -line.BaseQty,
                            line.BaseUomRef,
                            sourceLocationRef: line.SourceLocationRef,
                            oldQualityStatusRef: line.QualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));
                    break;

                case InventoryDocumentType.Adjustment:
                    var adjustmentDelta = line.AdjustmentDirection == InventoryAdjustmentDirection.Decrease
                        ? -line.BaseQty
                        : line.BaseQty;

                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            adjustmentDelta,
                            line.BaseUomRef,
                            sourceLocationRef: adjustmentDelta < 0 ? line.SourceLocationRef ?? line.DestinationLocationRef : null,
                            destinationLocationRef: adjustmentDelta > 0 ? line.DestinationLocationRef ?? line.SourceLocationRef : null,
                            oldQualityStatusRef: adjustmentDelta < 0 ? line.QualityStatusRef : null,
                            newQualityStatusRef: adjustmentDelta > 0 ? line.QualityStatusRef : null,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));
                    break;

                case InventoryDocumentType.Transfer:
                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            -line.BaseQty,
                            line.BaseUomRef,
                            sourceLocationRef: line.SourceLocationRef,
                            oldQualityStatusRef: line.QualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));

                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            line.BaseQty,
                            line.BaseUomRef,
                            destinationLocationRef: line.DestinationLocationRef,
                            newQualityStatusRef: line.QualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));
                    break;

                case InventoryDocumentType.QualityChange:
                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            -line.BaseQty,
                            line.BaseUomRef,
                            sourceLocationRef: line.SourceLocationRef ?? line.DestinationLocationRef,
                            oldQualityStatusRef: line.FromQualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));

                    yield return CreateEffect(
                        line,
                        InventoryTransactionLine.Create(
                            line.VariantRef,
                            line.Qty,
                            line.UomRef,
                            line.BaseQty,
                            line.BaseUomRef,
                            destinationLocationRef: line.DestinationLocationRef ?? line.SourceLocationRef,
                            newQualityStatusRef: line.ToQualityStatusRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode));
                    break;
            }
        }
    }

    private static InventoryDocumentPostingEffect CreateEffect(
        InventoryDocumentLine documentLine,
        InventoryTransactionLine transactionLine)
        => new(documentLine.BusinessKey.Value, transactionLine);

    private sealed record InventoryDocumentPostingEffect(
        Guid DocumentLineBusinessKey,
        InventoryTransactionLine TransactionLine);
}
