namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.PostInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
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
    private readonly ISerialItemCommandRepository _serialRepository;

    public PostInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventoryTransactionCommandRepository transactionRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialRepository)
    {
        _documentRepository = documentRepository;
        _transactionRepository = transactionRepository;
        _stockDetailRepository = stockDetailRepository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialRepository = serialRepository;
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

        var selectedSerialsByLine = BuildSelectedSerialsByLine(command.LineSerials);
        var effects = BuildEffects(document, selectedSerialsByLine).ToList();
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

            var serialResult = await HandleSerialItemsAsync(document, effect, stockDetail);
            if (!serialResult.Success)
                return Fail(serialResult.Error ?? "Serial item processing failed.");
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
            InventoryDocumentType.Conversion => InventoryTransactionType.Conversion,
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
                    effect.DocumentLine.BusinessKey.Value,
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
            InventoryDocumentType.Conversion => InventorySourceType.ConversionProduction,
            _ => null
        };
    }

    private static bool ShouldConsumeSource(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue or InventoryDocumentType.Adjustment or InventoryDocumentType.Conversion;

    private static IReadOnlyDictionary<Guid, PostLineSerialSelectionContext> BuildSelectedSerialsByLine(
        IReadOnlyCollection<PostInventoryDocumentLineSerialSelectionItem>? selections)
    {
        if (selections is null || selections.Count == 0)
        {
            return new Dictionary<Guid, PostLineSerialSelectionContext>();
        }

        return selections
            .Where(x => x.DocumentLineBusinessKey != Guid.Empty)
            .GroupBy(x => x.DocumentLineBusinessKey)
            .ToDictionary(
                group => group.Key,
                group => new PostLineSerialSelectionContext(
                    group.Any(x => x.UseUniqueSerialItems),
                    (IReadOnlyList<PostInventoryDocumentLineSerialItem>)group
                        .SelectMany(x => x.Serials ?? new List<PostInventoryDocumentLineSerialItem>())
                        .Where(x => x.SerialRef.HasValue || !string.IsNullOrWhiteSpace(x.SerialNo))
                        .ToList()));
    }

    private static IEnumerable<InventoryDocumentPostingEffect> BuildEffects(
        InventoryDocument document,
        IReadOnlyDictionary<Guid, PostLineSerialSelectionContext> selectedSerialsByLine)
    {
        foreach (var (line, index) in document.Lines.Select((line, index) => (line, index)))
        {
            selectedSerialsByLine.TryGetValue(line.BusinessKey.Value, out var selectedSerials);
            var serials = selectedSerials?.Serials ?? Array.Empty<PostInventoryDocumentLineSerialItem>();
            var serialRef = serials.FirstOrDefault(x => x.SerialRef.HasValue)?.SerialRef;

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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);

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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);

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
                            serialRef: serialRef,
                            lotBatchNo: line.LotBatchNo,
                            reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
                    break;

                case InventoryDocumentType.Conversion:
                    if (line.SourceLocationRef.HasValue)
                    {
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
                                serialRef: serialRef,
                                lotBatchNo: line.LotBatchNo,
                                reasonCode: line.ReasonCode),
                            serials,
                            selectedSerials?.UseUniqueSerialItems ?? false,
                            index + 1);
                    }

                    if (line.DestinationLocationRef.HasValue)
                    {
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
                                serialRef: serialRef,
                                lotBatchNo: line.LotBatchNo,
                                reasonCode: line.ReasonCode),
                            serials,
                            selectedSerials?.UseUniqueSerialItems ?? false,
                            index + 1);
                    }
                    break;
            }
        }
    }

    private static InventoryDocumentPostingEffect CreateEffect(
        InventoryDocumentLine documentLine,
        InventoryTransactionLine transactionLine,
        IReadOnlyList<PostInventoryDocumentLineSerialItem> serials,
        bool useUniqueSerialItems,
        int lineNo)
        => new(documentLine, transactionLine, serials, useUniqueSerialItems, lineNo);

    private async Task<(bool Success, string? Error)> HandleSerialItemsAsync(
        InventoryDocument document,
        InventoryDocumentPostingEffect effect,
        StockDetail stockDetail)
    {
        if (document.DocumentType != InventoryDocumentType.Receipt)
        {
            foreach (var selectedSerial in effect.Serials)
            {
                if (!selectedSerial.SerialRef.HasValue)
                    continue;

                var serialItem = await _serialRepository.GetByBusinessKeyAsync(selectedSerial.SerialRef.Value);
                if (serialItem is null)
                    return (false, "One of the selected serial items was not found.");

                serialItem.LinkStockDetail(stockDetail.BusinessKey);
            }

            return (true, null);
        }

        if (!effect.UseUniqueSerialItems && effect.Serials.Count == 0)
            return (true, null);

        var documentLine = effect.DocumentLine;
        if (documentLine.BaseQty <= 0)
            return (false, "Receipt line quantity is invalid for serial item generation.");

        if (documentLine.BaseQty != decimal.Truncate(documentLine.BaseQty))
            return (false, "Receipt line quantity must be a whole number when unique serial items are enabled.");

        var desiredSerialCount = (int)documentLine.BaseQty;
        var selectedSerialRefs = new HashSet<Guid>();

        foreach (var selectedSerial in effect.Serials)
        {
            if (!selectedSerial.SerialRef.HasValue)
                continue;

            var serialItem = await _serialRepository.GetByBusinessKeyAsync(selectedSerial.SerialRef.Value);
            if (serialItem is null)
                return (false, "One of the selected serial items was not found.");

            serialItem.LinkStockDetail(stockDetail.BusinessKey);
            selectedSerialRefs.Add(serialItem.BusinessKey.Value);
        }

        if (!effect.UseUniqueSerialItems)
            return (true, null);

        if (selectedSerialRefs.Count > desiredSerialCount)
            return (false, "Too many serial items were selected for this receipt line.");

        var remainingToGenerate = desiredSerialCount - selectedSerialRefs.Count;
        if (remainingToGenerate <= 0)
            return (true, null);

        for (var i = 0; i < remainingToGenerate; i++)
        {
            var serialNo = await GenerateUniqueReceiptSerialNoAsync(document, effect.LineNo, i + 1, documentLine.VariantRef);
            var serialItem = SerialItem.Create(
                serialNo,
                documentLine.VariantRef,
                document.SellerRef,
                document.WarehouseRef,
                stockDetail.LocationRef,
                stockDetail.QualityStatusRef,
                documentLine.LotBatchNo);

            await _serialRepository.InsertAsync(serialItem);
            serialItem.LinkStockDetail(stockDetail.BusinessKey);
        }

        return (true, null);
    }

    private async Task<string> GenerateUniqueReceiptSerialNoAsync(
        InventoryDocument document,
        int lineNo,
        int serialIndex,
        Guid variantRef)
    {
        var documentPart = NormalizeSerialSegment(document.DocumentNo);
        var candidateBase = $"SN-{documentPart}-L{lineNo:D2}-{serialIndex:D3}";
        var candidate = candidateBase;
        var suffix = 1;

        while (await _serialRepository.ExistsBySerialNoAsync(variantRef, candidate))
        {
            candidate = $"{candidateBase}-{suffix:D2}";
            suffix++;
        }

        return candidate;
    }

    private static string NormalizeSerialSegment(string value)
    {
        var normalized = value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return "DOC";

        normalized = normalized.Replace(' ', '-');
        normalized = normalized.Replace('/', '-');
        normalized = normalized.Replace('\\', '-');
        normalized = normalized.Replace(':', '-');
        normalized = normalized.Replace('.', '-');
        return normalized;
    }

    private sealed record InventoryDocumentPostingEffect(
        InventoryDocumentLine DocumentLine,
        InventoryTransactionLine TransactionLine,
        IReadOnlyList<PostInventoryDocumentLineSerialItem> Serials,
        bool UseUniqueSerialItems,
        int LineNo);

    private sealed record PostLineSerialSelectionContext(
        bool UseUniqueSerialItems,
        IReadOnlyList<PostInventoryDocumentLineSerialItem> Serials);
}
