namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.PostInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
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

    public PostInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventoryTransactionCommandRepository transactionRepository,
        IStockDetailCommandRepository stockDetailRepository)
    {
        _documentRepository = documentRepository;
        _transactionRepository = transactionRepository;
        _stockDetailRepository = stockDetailRepository;
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

        foreach (var line in BuildEffects(document))
        {
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

    private static IEnumerable<InventoryTransactionLine> BuildEffects(InventoryDocument document)
    {
        foreach (var line in document.Lines)
        {
            switch (document.DocumentType)
            {
                case InventoryDocumentType.Receipt:
                case InventoryDocumentType.Return:
                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        line.BaseQty,
                        line.BaseUomRef,
                        destinationLocationRef: line.DestinationLocationRef,
                        newQualityStatusRef: line.QualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);
                    break;

                case InventoryDocumentType.Issue:
                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        -line.BaseQty,
                        line.BaseUomRef,
                        sourceLocationRef: line.SourceLocationRef,
                        oldQualityStatusRef: line.QualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);
                    break;

                case InventoryDocumentType.Adjustment:
                    var adjustmentDelta = line.AdjustmentDirection == InventoryAdjustmentDirection.Decrease
                        ? -line.BaseQty
                        : line.BaseQty;

                    yield return InventoryTransactionLine.Create(
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
                        reasonCode: line.ReasonCode);
                    break;

                case InventoryDocumentType.Transfer:
                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        -line.BaseQty,
                        line.BaseUomRef,
                        sourceLocationRef: line.SourceLocationRef,
                        oldQualityStatusRef: line.QualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);

                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        line.BaseQty,
                        line.BaseUomRef,
                        destinationLocationRef: line.DestinationLocationRef,
                        newQualityStatusRef: line.QualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);
                    break;

                case InventoryDocumentType.QualityChange:
                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        -line.BaseQty,
                        line.BaseUomRef,
                        sourceLocationRef: line.SourceLocationRef ?? line.DestinationLocationRef,
                        oldQualityStatusRef: line.FromQualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);

                    yield return InventoryTransactionLine.Create(
                        line.VariantRef,
                        line.Qty,
                        line.UomRef,
                        line.BaseQty,
                        line.BaseUomRef,
                        destinationLocationRef: line.DestinationLocationRef ?? line.SourceLocationRef,
                        newQualityStatusRef: line.ToQualityStatusRef,
                        lotBatchNo: line.LotBatchNo,
                        reasonCode: line.ReasonCode);
                    break;
            }
        }
    }
}
