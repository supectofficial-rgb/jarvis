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
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
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
    private readonly ILocationQueryRepository _locationRepository;

    public PostInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventoryTransactionCommandRepository transactionRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialRepository,
        ILocationQueryRepository locationRepository)
    {
        _documentRepository = documentRepository;
        _transactionRepository = transactionRepository;
        _stockDetailRepository = stockDetailRepository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialRepository = serialRepository;
        _locationRepository = locationRepository;
    }

    public override async Task<CommandResult<PostInventoryDocumentCommandResult>> Handle(PostInventoryDocumentCommand command)
        => await ExecuteAsync(command);

    public async Task<CommandResult<PostInventoryDocumentCommandResult>> ExecuteAsync(PostInventoryDocumentCommand command)
    {
        var document = await _documentRepository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null || document.Id == 0)
            return Fail($"Inventory document '{command.DocumentBusinessKey:D}' was not found.");

        if (document.Status == InventoryDocumentStatus.Posted)
            return Fail($"Inventory document '{document.DocumentNo}' is already posted.");

        if (document.Lines.Count == 0)
            return Fail($"Inventory document '{document.DocumentNo}' ({document.BusinessKey.Value:D}) must contain at least one line before posting.");

        var resolvedTransactionWarehouseRef = await ResolveDocumentWarehouseRefAsync(document);

        if (resolvedTransactionWarehouseRef == Guid.Empty)
            return Fail($"Inventory document '{document.DocumentNo}' ({document.BusinessKey.Value:D}) does not resolve to a valid warehouse.");

        var transaction = InventoryTransaction.Create(
            transactionNo: $"TX-{document.DocumentNo}",
            transactionType: MapTransactionType(document.DocumentType),
            warehouseRef: resolvedTransactionWarehouseRef,
            sellerRef: document.SellerRef,
            occurredAt: document.OccurredAt,
            referenceType: nameof(InventoryDocument),
            referenceBusinessId: document.BusinessKey.Value,
            correlationId: document.CorrelationId,
            idempotencyKey: document.IdempotencyKey,
            reasonCode: document.ReasonCode);

        var receiptLotResolution = TryResolveReceiptLotBatchNo(document);
        if (!receiptLotResolution.Success)
            return Fail(receiptLotResolution.Error ?? "Unable to resolve receipt lot batch number.");

        if (document.DocumentType == InventoryDocumentType.Receipt)
        {
            document.ApplyReceiptLotBatchNo(receiptLotResolution.ReceiptLotBatchNo);
        }

        var selectedSerialsByLine = BuildSelectedSerialsByLine(document);
        var effects = BuildEffects(document, selectedSerialsByLine, receiptLotResolution.ReceiptLotBatchNo).ToList();
        foreach (var effect in effects)
        {
            var line = effect.TransactionLine;
            transaction.AddLine(line);
            var effectLabel = DescribeEffect(document, effect);
            var effectWarehouseRef = await ResolveEffectWarehouseRefAsync(effect);
            if (effectWarehouseRef == Guid.Empty)
                return Fail($"{effectLabel}: unable to resolve warehouse from document header or line locations.");

            var locationRef = line.BaseQtyDelta >= 0
                ? line.DestinationLocationRef
                : line.SourceLocationRef;

            var qualityStatusRef = line.BaseQtyDelta >= 0
                ? line.NewQualityStatusRef
                : line.OldQualityStatusRef;

            if (locationRef is null || qualityStatusRef is null)
                return Fail($"Posting failed for {effectLabel}: location and quality status must be resolved before posting.");

            var stockDetail = await _stockDetailRepository.FindByBucketAsync(
                line.VariantRef,
                document.SellerRef,
                effectWarehouseRef,
                locationRef.Value,
                qualityStatusRef.Value,
                line.LotBatchNo);

            if (stockDetail is null)
            {
                if (line.BaseQtyDelta < 0)
                    return Fail($"Posting failed for {effectLabel}: cannot issue or reduce stock because the source bucket does not exist.");

                stockDetail = StockDetail.Create(
                    line.VariantRef,
                    document.SellerRef,
                    effectWarehouseRef,
                    locationRef.Value,
                    qualityStatusRef.Value,
                    line.LotBatchNo,
                    0,
                    document.OccurredAt);

                await _stockDetailRepository.InsertAsync(stockDetail);
            }

            stockDetail.ApplyQuantity(line.BaseQtyDelta, document.OccurredAt);
            line.LinkStockDetail(stockDetail.BusinessKey);

            var serialResult = await HandleSerialItemsAsync(document, effect, stockDetail, effectWarehouseRef, transaction.BusinessKey);
            if (!serialResult.Success)
                return Fail($"{effectLabel}: {serialResult.Error ?? "Serial item processing failed."}");
        }

        var sourceTracingResult = await ApplySourceTracingAsync(document, transaction, effects);
        if (!sourceTracingResult.Success)
            return Fail($"Source tracing failed for inventory document '{document.DocumentNo}': {sourceTracingResult.Error ?? "Unknown source tracing error."}");

        transaction.MarkPosted();
        await _transactionRepository.InsertAsync(transaction);
        document.MarkPosted(transaction.BusinessKey, postedBy: null);
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
            InventoryDocumentType.ReturnFromSell => InventoryTransactionType.ReturnFromSell,
            InventoryDocumentType.ReturnFromBuy => InventoryTransactionType.ReturnFromBuy,
            InventoryDocumentType.ReturnFromTransfer => InventoryTransactionType.ReturnFromTransfer,
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
            var effectWarehouseRef = await ResolveEffectWarehouseRefAsync(effect);
            if (effectWarehouseRef == Guid.Empty)
                return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) does not resolve to a valid warehouse.");

            if (line.BaseQtyDelta > 0)
            {
                var sourceType = ResolveSourceType(document.DocumentType, line.BaseQtyDelta);
                if (sourceType is null)
                    continue;

                var locationRef = line.DestinationLocationRef;
                var qualityStatusRef = line.NewQualityStatusRef;
                if (locationRef is null || qualityStatusRef is null)
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) requires destination location and quality status to open a source balance.");

                var sourceBalance = InventorySourceBalance.Open(
                    sourceType.Value,
                    line.VariantRef,
                    document.SellerRef,
                    effectWarehouseRef,
                    locationRef.Value,
                    qualityStatusRef.Value,
                    line.BaseUomRef,
                    line.BaseQtyDelta,
                    line.LotBatchNo,
                    document.BusinessKey.Value,
                    effect.DocumentLine.BusinessKey.Value,
                    transaction.BusinessKey.Value,
                    line.BusinessKey.Value,
                    serialRef: null);

                await _sourceBalanceRepository.InsertAsync(sourceBalance);
                continue;
            }

            if (line.BaseQtyDelta < 0)
            {
                if (!ShouldConsumeSource(document.DocumentType))
                    continue;

                var qualityStatusRef = line.OldQualityStatusRef;
                if (qualityStatusRef is null)
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) requires quality status to consume a source balance.");

                var remainingToConsume = Math.Abs(line.BaseQtyDelta);
                var sourceBalances = await _sourceBalanceRepository.GetOpenByPoolAsync(
                    line.VariantRef,
                    effectWarehouseRef,
                    qualityStatusRef.Value,
                    line.LotBatchNo);

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
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) does not have enough open source balance to cover {Math.Abs(line.BaseQtyDelta)} base units.");
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
            InventoryDocumentType.ReturnFromSell => InventorySourceType.ReturnRestock,
            InventoryDocumentType.Adjustment => InventorySourceType.AdjustmentIncrease,
            InventoryDocumentType.Conversion => InventorySourceType.ConversionProduction,
            _ => null
        };
    }

    private static bool ShouldConsumeSource(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue
            or InventoryDocumentType.Adjustment
            or InventoryDocumentType.Conversion
            or InventoryDocumentType.ReturnFromBuy;

    private static IReadOnlyDictionary<Guid, PostLineSerialSelectionContext> BuildSelectedSerialsByLine(
        InventoryDocument document)
    {
        return document.Lines
            .Where(line => line.Serials.Count > 0)
            .ToDictionary(
                line => line.BusinessKey.Value,
                line => new PostLineSerialSelectionContext(
                    true,
                    (IReadOnlyList<PostInventoryDocumentLineSerialItem>)line.Serials
                        .Select(serial => new PostInventoryDocumentLineSerialItem
                        {
                            SerialRef = serial.SerialRef,
                            SerialNo = serial.SerialNo
                        })
                        .ToList()));
    }

    private static IEnumerable<InventoryDocumentPostingEffect> BuildEffects(
        InventoryDocument document,
        IReadOnlyDictionary<Guid, PostLineSerialSelectionContext> selectedSerialsByLine,
        string? receiptLotBatchNo)
    {
        foreach (var (line, index) in document.Lines.Select((line, index) => (line, index)))
        {
            selectedSerialsByLine.TryGetValue(line.BusinessKey.Value, out var selectedSerials);
            var serials = selectedSerials?.Serials ?? Array.Empty<PostInventoryDocumentLineSerialItem>();
            var serialTuples = serials
                .Where(x => x.SerialRef.HasValue)
                .Select(x => (x.SerialRef, x.SerialNo))
                .ToList();
            var serialRef = serialTuples.FirstOrDefault().SerialRef;
            var lotBatchNo = ResolveDocumentLotBatchNo(document, line, receiptLotBatchNo);

            switch (document.DocumentType)
            {
                case InventoryDocumentType.Receipt:
                case InventoryDocumentType.ReturnFromSell:
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
                              reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
                    break;

                case InventoryDocumentType.ReturnFromBuy:
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
                              reasonCode: line.ReasonCode),
                        serials,
                        selectedSerials?.UseUniqueSerialItems ?? false,
                        index + 1);
                    break;

                case InventoryDocumentType.ReturnFromTransfer:
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
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
                              serials: serialTuples,
                              lotBatchNo: lotBatchNo,
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
                                serials: serialTuples,
                                lotBatchNo: lotBatchNo,
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
                                serials: serialTuples,
                                lotBatchNo: lotBatchNo,
                                reasonCode: line.ReasonCode),
                            serials,
                            selectedSerials?.UseUniqueSerialItems ?? false,
                            index + 1);
                    }
                    break;
            }
        }
    }

    private static (bool Success, string? Error, string? ReceiptLotBatchNo) TryResolveReceiptLotBatchNo(InventoryDocument document)
    {
        if (document.DocumentType != InventoryDocumentType.Receipt)
            return (true, null, null);

        var lots = document.Lines
            .Select(line => NormalizeLotBatchNo(line.LotBatchNo))
            .Where(lot => !string.IsNullOrWhiteSpace(lot))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lots.Count > 1)
        {
            return (
                false,
                $"Receipt '{document.DocumentNo}' contains multiple lot batch numbers. Use a single lot batch number for the whole receipt or leave them empty to auto-generate one.",
                null);
        }

        if (lots.Count == 1)
            return (true, null, lots[0]);

        return (true, null, GenerateReceiptLotBatchNo(document));
    }

    private static string? ResolveDocumentLotBatchNo(
        InventoryDocument document,
        InventoryDocumentLine line,
        string? receiptLotBatchNo)
    {
        if (document.DocumentType == InventoryDocumentType.Receipt)
            return NormalizeLotBatchNo(line.LotBatchNo) ?? receiptLotBatchNo ?? GenerateReceiptLotBatchNo(document);

        return NormalizeLotBatchNo(line.LotBatchNo) ?? receiptLotBatchNo;
    }

    private static InventoryDocumentPostingEffect CreateEffect(
        InventoryDocumentLine documentLine,
        InventoryTransactionLine transactionLine,
        IReadOnlyList<PostInventoryDocumentLineSerialItem> serials,
        bool useUniqueSerialItems,
        int lineNo)
        => new(documentLine, transactionLine, serials, useUniqueSerialItems, lineNo);

    private static string DescribeEffect(InventoryDocument document, InventoryDocumentPostingEffect effect)
        => $"inventory document '{document.DocumentNo}' ({document.BusinessKey.Value:D}, {document.DocumentType}) line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}, variant {effect.DocumentLine.VariantRef:D})";

    private async Task<(bool Success, string? Error)> HandleSerialItemsAsync(
        InventoryDocument document,
        InventoryDocumentPostingEffect effect,
        StockDetail stockDetail,
        Guid warehouseRef,
        BusinessKey transactionBusinessKey)
    {
        var shouldGenerateSerials = document.DocumentType == InventoryDocumentType.Receipt
            || (document.DocumentType == InventoryDocumentType.Conversion && effect.DocumentLine.BaseQty > 0);

        if (!shouldGenerateSerials)
        {
            foreach (var selectedSerial in effect.Serials)
            {
                if (!selectedSerial.SerialRef.HasValue)
                    continue;

                var serialItem = await _serialRepository.GetByBusinessKeyAsync(selectedSerial.SerialRef.Value);
                if (serialItem is null)
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) references serial item '{selectedSerial.SerialRef.Value:D}' which was not found.");

                serialItem.LinkStockDetail(stockDetail.BusinessKey);
                var serialTransitionResult = ApplySerialPostingTransition(
                    document,
                    effect,
                    serialItem,
                    stockDetail,
                    transactionBusinessKey);
                if (!serialTransitionResult.Success)
                    return serialTransitionResult;
            }

            return (true, null);
        }

        if (!effect.UseUniqueSerialItems && effect.Serials.Count == 0)
            return (true, null);

        var documentLine = effect.DocumentLine;
        if (effect.UseUniqueSerialItems)
        {
            if (documentLine.BaseQty <= 0)
                return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({documentLine.VariantRef:D}) has invalid quantity for serial item generation.");

            if (documentLine.BaseQty != decimal.Truncate(documentLine.BaseQty))
                return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({documentLine.VariantRef:D}) must have a whole base quantity when unique serial items are enabled.");
        }

        var createdOrLinkedSerialCount = 0;
        foreach (var selectedSerial in effect.Serials)
        {
            if (selectedSerial.SerialRef.HasValue)
            {
                var serialItem = await _serialRepository.GetByBusinessKeyAsync(selectedSerial.SerialRef.Value);
                if (serialItem is null)
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) references serial item '{selectedSerial.SerialRef.Value:D}' which was not found.");

                serialItem.LinkStockDetail(stockDetail.BusinessKey);
                var serialTransitionResult = ApplySerialPostingTransition(
                    document,
                    effect,
                    serialItem,
                    stockDetail,
                    transactionBusinessKey);
                if (!serialTransitionResult.Success)
                    return serialTransitionResult;
                createdOrLinkedSerialCount++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(selectedSerial.SerialNo))
                return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) is missing a serial number for serial item generation.");

            var generatedSerialItem = SerialItem.Create(
                selectedSerial.SerialNo,
                documentLine.VariantRef,
                document.SellerRef,
                warehouseRef,
                stockDetail.LocationRef,
                stockDetail.QualityStatusRef,
                stockDetail.LotBatchNo);

            await _serialRepository.InsertAsync(generatedSerialItem);
            generatedSerialItem.LinkStockDetail(stockDetail.BusinessKey);
            effect.TransactionLine.AddSerial(generatedSerialItem.BusinessKey.Value, generatedSerialItem.SerialNo);
            createdOrLinkedSerialCount++;
        }

        if (!effect.UseUniqueSerialItems)
            return (true, null);

        var desiredSerialCount = (int)documentLine.BaseQty;
        if (createdOrLinkedSerialCount > desiredSerialCount)
            return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({documentLine.BusinessKey.Value:D}) has more selected serial items than base quantity allows.");

        var remainingToGenerate = desiredSerialCount - createdOrLinkedSerialCount;
        if (remainingToGenerate <= 0)
            return (true, null);

        for (var i = 0; i < remainingToGenerate; i++)
        {
            var serialNo = await GenerateUniqueReceiptSerialNoAsync(document, effect.LineNo, createdOrLinkedSerialCount + i + 1, documentLine.VariantRef);
            var serialItem = SerialItem.Create(
                serialNo,
                documentLine.VariantRef,
                document.SellerRef,
                warehouseRef,
                stockDetail.LocationRef,
                stockDetail.QualityStatusRef,
                stockDetail.LotBatchNo);

            await _serialRepository.InsertAsync(serialItem);
            serialItem.LinkStockDetail(stockDetail.BusinessKey);
            effect.TransactionLine.AddSerial(serialItem.BusinessKey.Value, serialNo);
        }

        return (true, null);
    }

    private static (bool Success, string? Error) ApplySerialPostingTransition(
        InventoryDocument document,
        InventoryDocumentPostingEffect effect,
        SerialItem serialItem,
        StockDetail stockDetail,
        BusinessKey transactionBusinessKey)
    {
        try
        {
            switch (document.DocumentType)
            {
                case InventoryDocumentType.Receipt:
                case InventoryDocumentType.ReturnFromSell:
                    serialItem.ReturnToAvailable(
                        stockDetail.WarehouseRef,
                        stockDetail.LocationRef,
                        stockDetail.QualityStatusRef,
                        stockDetail.LotBatchNo);
                    break;

                case InventoryDocumentType.Issue:
                case InventoryDocumentType.ReturnFromBuy:
                    if (effect.TransactionLine.BaseQtyDelta < 0)
                        serialItem.Issue(transactionBusinessKey);
                    break;

                case InventoryDocumentType.Transfer:
                case InventoryDocumentType.ReturnFromTransfer:
                    if (effect.TransactionLine.BaseQtyDelta > 0)
                    {
                        serialItem.Move(
                            stockDetail.WarehouseRef,
                            stockDetail.LocationRef,
                            stockDetail.QualityStatusRef,
                            stockDetail.LotBatchNo);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
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

    private static string GenerateReceiptLotBatchNo(InventoryDocument document)
    {
        var documentPart = NormalizeSerialSegment(string.IsNullOrWhiteSpace(document.DocumentNo)
            ? document.BusinessKey.Value.ToString("N")
            : document.DocumentNo);

        return $"LOT-{documentPart}";
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

    private static string? NormalizeLotBatchNo(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private async Task<Guid> ResolveDocumentWarehouseRefAsync(InventoryDocument document)
    {
        foreach (var line in document.Lines)
        {
            var lineWarehouseRef = await ResolveLineWarehouseRefAsync(line);
            if (lineWarehouseRef != Guid.Empty)
                return lineWarehouseRef;
        }

        return Guid.Empty;
    }

    private async Task<Guid> ResolveEffectWarehouseRefAsync(InventoryDocumentPostingEffect effect)
    {
        var line = effect.DocumentLine;
        return await ResolveLineWarehouseRefAsync(line);
    }

    private async Task<Guid> ResolveLineWarehouseRefAsync(InventoryDocumentLine line)
    {
        var locationRef = line.DestinationLocationRef ?? line.SourceLocationRef;
        if (!locationRef.HasValue)
            return Guid.Empty;

        return await ResolveLocationWarehouseRefAsync(locationRef.Value);
    }

    private async Task<Guid> ResolveLocationWarehouseRefAsync(Guid locationBusinessKey)
    {
        var location = await _locationRepository.GetByBusinessKeyAsync(locationBusinessKey);
        return location?.WarehouseRef ?? Guid.Empty;
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
