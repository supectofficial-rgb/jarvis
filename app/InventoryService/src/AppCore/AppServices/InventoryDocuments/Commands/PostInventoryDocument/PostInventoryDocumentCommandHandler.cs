namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.PostInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;

public class PostInventoryDocumentCommandHandler
    : CommandHandler<PostInventoryDocumentCommand, PostInventoryDocumentCommandResult>
{
    private readonly IInventoryDocumentCommandRepository _documentRepository;
    private readonly IInventoryTransactionCommandRepository _transactionRepository;
    private readonly IStockDetailCommandRepository _stockDetailRepository;
    private readonly IStockDetailQueryRepository _stockDetailQueryRepository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;
    private readonly ISerialItemCommandRepository _serialRepository;
    private readonly ISerialItemQueryRepository _serialQueryRepository;
    private readonly ILocationQueryRepository _locationRepository;
    private readonly ILogger<PostInventoryDocumentCommandHandler> _logger;

    public PostInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventoryTransactionCommandRepository transactionRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IStockDetailQueryRepository stockDetailQueryRepository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialRepository,
        ISerialItemQueryRepository serialQueryRepository,
        ILocationQueryRepository locationRepository,
        ILogger<PostInventoryDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _transactionRepository = transactionRepository;
        _stockDetailRepository = stockDetailRepository;
        _stockDetailQueryRepository = stockDetailQueryRepository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialRepository = serialRepository;
        _serialQueryRepository = serialQueryRepository;
        _locationRepository = locationRepository;
        _logger = logger;
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

        var receiptLotValidation = ValidateReceiptLotBatchNumbers(document);
        if (!receiptLotValidation.Success)
            return Fail(receiptLotValidation.Error ?? "Unable to validate receipt lot batch number.");

        var selectedSerialsByLine = BuildSelectedSerialsByLine(document);
        var effects = BuildEffects(document, selectedSerialsByLine).ToList();
        _logger.LogInformation(
            "Posting inventory document {DocumentNo} ({DocumentBusinessKey}) type {DocumentType} with {LineCount} effects.",
            document.DocumentNo,
            document.BusinessKey.Value,
            document.DocumentType,
            effects.Count);

        foreach (var effect in effects)
        {
            var line = effect.TransactionLine;
            transaction.AddLine(line);
            var effectLabel = DescribeEffect(document, effect);
            _logger.LogInformation(
                "Processing effect {EffectLabel} for document {DocumentNo} line {LineNo} ({DocumentLineBusinessKey}) qtyDelta {BaseQtyDelta} sourceLocation {SourceLocationRef} destinationLocation {DestinationLocationRef} lot {LotBatchNo} quality {OldQualityStatusRef}->{NewQualityStatusRef}.",
                effectLabel,
                document.DocumentNo,
                effect.LineNo,
                effect.DocumentLine.BusinessKey.Value,
                line.BaseQtyDelta,
                line.SourceLocationRef,
                line.DestinationLocationRef,
                line.LotBatchNo,
                line.OldQualityStatusRef,
                line.NewQualityStatusRef);

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

        _logger.LogInformation(
            "Applying source tracing for inventory document {DocumentNo} ({DocumentBusinessKey}) with {EffectCount} effects.",
            document.DocumentNo,
            document.BusinessKey.Value,
            effects.Count);

        var sourceTracingResult = await ApplySourceTracingAsync(document, transaction, effects);
        if (!sourceTracingResult.Success)
        {
            _logger.LogWarning(
                "Source tracing failed for inventory document {DocumentNo} ({DocumentBusinessKey}): {ErrorMessage}",
                document.DocumentNo,
                document.BusinessKey.Value,
                sourceTracingResult.Error);
            return Fail($"Source tracing failed for inventory document '{document.DocumentNo}': {sourceTracingResult.Error ?? "Unknown source tracing error."}");
        }

        _logger.LogInformation(
            "Source tracing completed for inventory document {DocumentNo} ({DocumentBusinessKey}).",
            document.DocumentNo,
            document.BusinessKey.Value);

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
                var reservationRef = effect.DocumentLine.BusinessKey.Value;

                var allocatedSourceBalances = await _sourceBalanceRepository.GetByReservationRefAsync(reservationRef);
                foreach (var sourceBalance in allocatedSourceBalances
                             .OrderBy(x => x.OpenedAt)
                             .ThenBy(x => x.Id))
                {
                    if (remainingToConsume <= 0)
                        break;

                    var allocations = sourceBalance.Allocations
                        .Where(x => x.ReservationRef == reservationRef && x.ActiveAllocatedQty > 0)
                        .OrderBy(x => x.CreatedAt)
                        .ThenBy(x => x.Id)
                        .ToList();

                    foreach (var allocation in allocations)
                    {
                        if (remainingToConsume <= 0)
                            break;

                        var consumedQty = Math.Min(allocation.ActiveAllocatedQty, remainingToConsume);
                        if (consumedQty <= 0)
                            continue;

                        sourceBalance.ConsumeAllocated(
                            allocation.BusinessKey.Value,
                            consumedQty,
                            transaction.BusinessKey.Value,
                            effect.DocumentLine.BusinessKey.Value,
                            line.ReasonCode ?? document.ReasonCode);

                        remainingToConsume -= consumedQty;
                    }
                }

                if (remainingToConsume > 0)
                {
                    return (false, $"document '{document.DocumentNo}' line {effect.LineNo} ({effect.DocumentLine.BusinessKey.Value:D}) does not have enough open source balance to cover {Math.Abs(line.BaseQtyDelta)} base units.");
                }
            }
        }

        return (true, null);
    }

    private async Task<(bool Success, string? Error)> EnsureAutoAllocatedSerialsAsync(InventoryDocument document)
    {
        if (!ShouldAutoAllocateSerials(document.DocumentType))
            return (true, null);

        var originalLines = document.Lines.ToList();
        foreach (var (line, lineIndex) in originalLines.Select((line, index) => (line, index + 1)))
        {
            if (line.Serials.Count > 0)
                continue;

            if (!line.SourceLocationRef.HasValue)
                continue;

            if (line.BaseQty <= 0 || line.BaseQty != decimal.Truncate(line.BaseQty))
                continue;

            var lineWarehouseRef = await ResolveLineWarehouseRefAsync(line);
            if (lineWarehouseRef == Guid.Empty)
                continue;

            var allocationResult = await BuildAutoAllocationPlanAsync(document.DocumentNo, line, lineIndex, lineWarehouseRef);
            if (!allocationResult.Success)
                return (false, allocationResult.Error);

            if (allocationResult.Allocations.Count == 0)
                continue;

            ApplyAutoAllocatedSerials(document, line, allocationResult.Allocations);
        }

        return (true, null);
    }

    private async Task<(bool Success, string? Error, IReadOnlyList<AutoAllocatedBucketChunk> Allocations)> BuildAutoAllocationPlanAsync(
        string documentNo,
        InventoryDocumentLine line,
        int lineNo,
        Guid warehouseRef)
    {
        var normalizedLotBatchNo = NormalizeLotBatchNo(line.LotBatchNo);
        var availableBuckets = await LoadAvailableBucketsForAutoAllocationAsync(line, warehouseRef);
        if (normalizedLotBatchNo is not null)
        {
            availableBuckets = availableBuckets
                .Where(x => string.Equals(NormalizeLotBatchNo(x.LotBatchNo), normalizedLotBatchNo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (availableBuckets.Count == 0)
        {
            return (
                false,
                $"document '{documentNo}' line {lineNo} ({line.BusinessKey.Value:D}) does not have enough available source buckets to auto-allocate {line.BaseQty} base units.",
                Array.Empty<AutoAllocatedBucketChunk>());
        }

        var remainingBaseQty = line.BaseQty;
        var allocations = new List<AutoAllocatedBucketChunk>();
        foreach (var bucket in availableBuckets)
        {
            if (remainingBaseQty <= 0)
                break;

            var bucketQtyToConsume = Math.Min(bucket.QuantityOnHand, remainingBaseQty);
            if (bucketQtyToConsume <= 0)
                continue;

            var availableSerials = await LoadAvailableSerialsForBucketAsync(bucket.StockDetailBusinessKey);
            if (availableSerials.Count == 0)
            {
                allocations.Add(new AutoAllocatedBucketChunk(bucket, bucketQtyToConsume, Array.Empty<SerialItemListItem>()));
                remainingBaseQty -= bucketQtyToConsume;
                continue;
            }

            var serialQtyToConsume = (int)Math.Min(bucketQtyToConsume, availableSerials.Count);
            if (serialQtyToConsume > 0)
            {
                allocations.Add(new AutoAllocatedBucketChunk(
                    bucket,
                    serialQtyToConsume,
                    availableSerials.Take(serialQtyToConsume).ToList()));
                remainingBaseQty -= serialQtyToConsume;
                bucketQtyToConsume -= serialQtyToConsume;
            }

            if (bucketQtyToConsume > 0)
            {
                allocations.Add(new AutoAllocatedBucketChunk(bucket, bucketQtyToConsume, Array.Empty<SerialItemListItem>()));
                remainingBaseQty -= bucketQtyToConsume;
            }
        }

        if (remainingBaseQty > 0)
        {
            return (
                false,
                $"document '{documentNo}' line {lineNo} ({line.BusinessKey.Value:D}) does not have enough available source buckets to auto-allocate {line.BaseQty} base units.",
                Array.Empty<AutoAllocatedBucketChunk>());
        }

        return (true, null, allocations);
    }

    private static bool ShouldAutoAllocateSerials(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue
            or InventoryDocumentType.Transfer
            or InventoryDocumentType.ReturnFromBuy
            or InventoryDocumentType.ReturnFromTransfer;

    private async Task<IReadOnlyList<StockDetailListItem>> LoadAvailableBucketsForAutoAllocationAsync(
        InventoryDocumentLine line,
        Guid warehouseRef)
    {
        var query = new GetAvailableStockBucketsQuery
        {
            VariantRef = line.VariantRef,
            WarehouseRef = warehouseRef,
            LocationRef = line.SourceLocationRef,
            QualityStatusRef = line.QualityStatusRef
        };

        var buckets = await _stockDetailQueryRepository.GetAvailableBucketsAsync(query);

        if (buckets.Count == 0 && line.QualityStatusRef.HasValue)
        {
            query.QualityStatusRef = null;
            buckets = await _stockDetailQueryRepository.GetAvailableBucketsAsync(query);
        }

        return buckets
            .OrderBy(x => x.FirstReceivedAt)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.StockDetailBusinessKey)
            .ToList();
    }

    private async Task<IReadOnlyList<SerialItemListItem>> LoadAvailableSerialsForBucketAsync(Guid stockDetailBusinessKey)
    {
        var items = new List<SerialItemListItem>();
        var page = 1;

        while (true)
        {
            var result = await _serialQueryRepository.SearchAsync(new SearchSerialItemsQuery
            {
                StockDetailRef = stockDetailBusinessKey,
                Status = SerialItemStatus.Available.ToString(),
                Page = page,
                PageSize = 200
            });

            var pageItems = result.Items ?? new List<SerialItemListItem>();
            if (pageItems.Count == 0)
                break;

            items.AddRange(pageItems);

            var returnedPageSize = result.PageSize <= 0 ? 200 : result.PageSize;
            if (items.Count >= result.TotalCount || pageItems.Count < returnedPageSize)
                break;

            page++;
        }

        return items
            .OrderBy(x => x.DateScannedIn)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.SerialNo, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.SerialItemBusinessKey)
            .ToList();
    }

    private static void ApplyAutoAllocatedSerials(
        InventoryDocument document,
        InventoryDocumentLine line,
        IReadOnlyList<AutoAllocatedBucketChunk> allocations)
    {
        if (allocations.Count == 0)
            return;

        var groupedAllocations = allocations
            .GroupBy(x => NormalizeLotBatchNo(x.Bucket.LotBatchNo) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (groupedAllocations.Count <= 1)
        {
            var selectedAllocation = groupedAllocations[0].First();
            var selectedLotBatchNo = NormalizeLotBatchNo(selectedAllocation.Bucket.LotBatchNo);
            line.Update(
                line.VariantRef,
                line.Qty,
                line.UomRef,
                line.BaseQty,
                line.BaseUomRef,
                line.SourceLocationRef,
                line.DestinationLocationRef,
                selectedAllocation.Bucket.QualityStatusRef,
                line.FromQualityStatusRef,
                line.ToQualityStatusRef,
                string.IsNullOrWhiteSpace(line.LotBatchNo) ? selectedLotBatchNo : line.LotBatchNo,
                line.ReasonCode,
                line.AdjustmentDirection);

            foreach (var allocation in allocations)
            {
                foreach (var selectedSerial in allocation.Serials)
                {
                    line.AddSerial(selectedSerial.SerialItemBusinessKey, selectedSerial.SerialNo);
                }
            }

            return;
        }

        document.RemoveLine(line.BusinessKey.Value);

        var qtyPerBaseUnit = line.Qty / line.BaseQty;
        foreach (var group in groupedAllocations)
        {
            var groupBaseQty = group.Sum(x => x.BaseQty);
            var firstAllocation = group.First();
            var groupLotBatchNo = NormalizeLotBatchNo(group.Key);
            var splitLine = InventoryDocumentLine.Create(
                line.VariantRef,
                decimal.Round(qtyPerBaseUnit * groupBaseQty, 6, MidpointRounding.AwayFromZero),
                line.UomRef,
                groupBaseQty,
                line.BaseUomRef,
                line.SourceLocationRef,
                line.DestinationLocationRef,
                firstAllocation.Bucket.QualityStatusRef,
                line.FromQualityStatusRef,
                line.ToQualityStatusRef,
                groupLotBatchNo,
                line.ReasonCode,
                line.AdjustmentDirection);

            foreach (var allocation in group)
            {
                foreach (var selectedSerial in allocation.Serials)
                {
                    splitLine.AddSerial(selectedSerial.SerialItemBusinessKey, selectedSerial.SerialNo);
                }
            }

            document.AddLine(splitLine);
        }
    }

    private sealed record AutoAllocatedBucketChunk(
        StockDetailListItem Bucket,
        decimal BaseQty,
        IReadOnlyList<SerialItemListItem> Serials);

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
            or InventoryDocumentType.Transfer
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
        IReadOnlyDictionary<Guid, PostLineSerialSelectionContext> selectedSerialsByLine)
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
            var lotBatchNo = NormalizeLotBatchNo(line.LotBatchNo);

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

    private static (bool Success, string? Error) ValidateReceiptLotBatchNumbers(InventoryDocument document)
    {
        if (document.DocumentType != InventoryDocumentType.Receipt)
            return (true, null);

        var lots = document.Lines
            .Select(line => NormalizeLotBatchNo(line.LotBatchNo))
            .Where(lot => !string.IsNullOrWhiteSpace(lot))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lots.Count == 1 && document.Lines.All(line => string.Equals(NormalizeLotBatchNo(line.LotBatchNo), lots[0], StringComparison.OrdinalIgnoreCase)))
            return (true, null);

        if (lots.Count == 0)
        {
            return (
                false,
                $"Receipt '{document.DocumentNo}' must have a lot batch number on its lines before posting.");
        }

        return (
            false,
            $"Receipt '{document.DocumentNo}' contains multiple lot batch numbers. Use a single lot batch number for the whole receipt.");
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
