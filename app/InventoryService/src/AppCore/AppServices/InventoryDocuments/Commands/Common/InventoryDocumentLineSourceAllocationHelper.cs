namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using OysterFx.AppCore.Domain.Exceptions;
using Microsoft.Extensions.Logging;

internal static class InventoryDocumentLineSourceAllocationHelper
{
    public static bool ShouldReserveSourceBalances(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue
            or InventoryDocumentType.Transfer;

    public static async Task ReleaseSourceAllocationsAsync(
        Guid reservationRef,
        IInventorySourceBalanceCommandRepository repository)
    {
        var sourceBalances = await repository.GetByReservationRefAsync(reservationRef);
        foreach (var sourceBalance in sourceBalances)
        {
            var allocations = sourceBalance.Allocations
                .Where(x => x.ReservationRef == reservationRef && x.ActiveAllocatedQty > 0)
                .ToList();

            foreach (var allocation in allocations)
            {
                sourceBalance.ReleaseAllocation(allocation.BusinessKey.Value, allocation.ActiveAllocatedQty);
            }
        }
    }

    public static async Task AllocateSourceBalancesAsync(
        InventoryDocument document,
        InventoryDocumentLine line,
        IInventorySourceBalanceCommandRepository repository,
        ILogger? logger = null)
    {
        if (!ShouldReserveSourceBalances(document.DocumentType))
        {
            return;
        }

        if (!line.SourceLocationRef.HasValue)
        {
            throw new AggregateStateExceptions("Source location is required for source allocation.", nameof(line.SourceLocationRef));
        }

        if (line.BaseQty <= 0)
        {
            return;
        }

        var selectedSerialRefs = line.Serials
            .Where(x => x.SerialRef.HasValue)
            .Select(x => x.SerialRef!.Value)
            .ToHashSet();
        var requestedLotBatchNo = NormalizeLotBatchNo(line.LotBatchNo);
        var requestedQualityStatusRef = line.QualityStatusRef;

        logger?.LogWarning(
            "Allocating source balances for document {DocumentNo} ({DocumentBusinessKey}) line {LineBusinessKey} variant {VariantRef} qty {BaseQty} sourceLocation {SourceLocationRef} lot {LotBatchNo} quality {QualityStatusRef} serialCount {SerialCount}.",
            document.DocumentNo,
            document.BusinessKey.Value,
            line.BusinessKey.Value,
            line.VariantRef,
            line.BaseQty,
            line.SourceLocationRef,
            requestedLotBatchNo,
            requestedQualityStatusRef,
            selectedSerialRefs.Count);

        if (selectedSerialRefs.Count > 0)
        {
            logger?.LogWarning(
                "Selected serial refs for line {LineBusinessKey}: {SerialRefs}.",
                line.BusinessKey.Value,
                string.Join(", ", selectedSerialRefs.Select(x => x.ToString("D"))));
        }

        var sourceBalances = await repository.GetOpenByPoolAsync(
            line.VariantRef,
            document.WarehouseRef);

        var orderedSourceBalances = sourceBalances
            .Where(x => x.LocationRef == line.SourceLocationRef.Value)
            .OrderByDescending(x => selectedSerialRefs.Count > 0 && x.SerialRef.HasValue && selectedSerialRefs.Contains(x.SerialRef.Value))
            .ThenByDescending(x => !string.IsNullOrWhiteSpace(requestedLotBatchNo) && string.Equals(NormalizeLotBatchNo(x.LotBatchNo), requestedLotBatchNo, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => requestedQualityStatusRef.HasValue && x.QualityStatusRef == requestedQualityStatusRef.Value)
            .ThenBy(x => x.OpenedAt)
            .ThenBy(x => x.Id)
            .ToList();

        logger?.LogWarning(
            "Source allocation candidate count for line {LineBusinessKey}: totalOpen={TotalOpen}, scopedCandidates={ScopedCandidates}, selectedSerials={SelectedSerials}.",
            line.BusinessKey.Value,
            sourceBalances.Count,
            orderedSourceBalances.Count,
            selectedSerialRefs.Count);

        if (selectedSerialRefs.Count > 0)
        {
            var matchingSelectedSerialBalances = orderedSourceBalances
                .Where(x => x.SerialRef.HasValue && selectedSerialRefs.Contains(x.SerialRef.Value))
                .ToList();

            logger?.LogWarning(
                "Source allocation selected-serial matches for line {LineBusinessKey}: matchingCandidates={MatchingCandidates}, selectedSerials={SelectedSerials}, sourceLocation={SourceLocationRef}, lot={LotBatchNo}, quality={QualityStatusRef}.",
                line.BusinessKey.Value,
                matchingSelectedSerialBalances.Count,
                selectedSerialRefs.Count,
                line.SourceLocationRef,
                requestedLotBatchNo,
                requestedQualityStatusRef);

            foreach (var matchedCandidate in matchingSelectedSerialBalances.Take(10))
            {
                logger?.LogWarning(
                    "Matched selected serial source balance for line {LineBusinessKey}: sourceBalance {SourceBalanceBusinessKey} serial {SerialRef} location {LocationRef} lot {LotBatchNo} quality {QualityStatusRef} available {AvailableQty} openedAt {OpenedAt}.",
                    line.BusinessKey.Value,
                    matchedCandidate.BusinessKey.Value,
                    matchedCandidate.SerialRef,
                    matchedCandidate.LocationRef,
                    matchedCandidate.LotBatchNo,
                    matchedCandidate.QualityStatusRef,
                    matchedCandidate.AvailableQty,
                    matchedCandidate.OpenedAt);
            }
        }

        foreach (var candidate in orderedSourceBalances.Take(10))
        {
            logger?.LogWarning(
                "Source allocation candidate for line {LineBusinessKey}: sourceBalance {SourceBalanceBusinessKey} location {LocationRef} lot {LotBatchNo} quality {QualityStatusRef} serial {SerialRef} available {AvailableQty} openedAt {OpenedAt}.",
                line.BusinessKey.Value,
                candidate.BusinessKey.Value,
                candidate.LocationRef,
                candidate.LotBatchNo,
                candidate.QualityStatusRef,
                candidate.SerialRef,
                candidate.AvailableQty,
                candidate.OpenedAt);
        }

        if (orderedSourceBalances.Count == 0)
        {
            logger?.LogWarning(
                "No open source balance found for line {LineBusinessKey} on document {DocumentNo}. sourceLocation={SourceLocationRef} lot={LotBatchNo} quality={QualityStatusRef} selectedSerials={SelectedSerials}.",
                line.BusinessKey.Value,
                document.DocumentNo,
                line.SourceLocationRef,
                requestedLotBatchNo,
                requestedQualityStatusRef,
                string.Join(", ", selectedSerialRefs.Select(x => x.ToString("D"))));
            throw new AggregateStateExceptions("No open source balance found for the selected line.", nameof(line.VariantRef));
        }

        var remaining = line.BaseQty;
        foreach (var sourceBalance in orderedSourceBalances)
        {
            if (remaining <= 0)
            {
                break;
            }

            var quantityToAllocate = Math.Min(sourceBalance.AvailableQty, remaining);
            if (quantityToAllocate <= 0)
            {
                logger?.LogWarning(
                    "Skipping sourceBalance {SourceBalanceBusinessKey} for line {LineBusinessKey} because available qty is {AvailableQty}.",
                    sourceBalance.BusinessKey.Value,
                    line.BusinessKey.Value,
                    sourceBalance.AvailableQty);
                continue;
            }

            sourceBalance.Allocate(line.BusinessKey.Value, sourceBalance.BusinessKey.Value, quantityToAllocate);
            logger?.LogWarning(
                "Allocated {QuantityToAllocate} from sourceBalance {SourceBalanceBusinessKey} to line {LineBusinessKey}; remaining {Remaining}.",
                quantityToAllocate,
                sourceBalance.BusinessKey.Value,
                line.BusinessKey.Value,
                remaining - quantityToAllocate);
            remaining -= quantityToAllocate;
        }

        if (remaining > 0)
        {
            logger?.LogWarning(
                "Source allocation failed for line {LineBusinessKey} on document {DocumentNo}: requested {RequestedQty}, remaining {RemainingQty}.",
                line.BusinessKey.Value,
                document.DocumentNo,
                line.BaseQty,
                remaining);
            throw new AggregateStateExceptions(
                $"document line '{line.BusinessKey.Value:D}' does not have enough open source balance to cover {line.BaseQty} base units.",
                nameof(line.BaseQty));
        }

        logger?.LogWarning(
            "Source allocation completed for line {LineBusinessKey} on document {DocumentNo}.",
            line.BusinessKey.Value,
            document.DocumentNo);
    }

    private static string? NormalizeLotBatchNo(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
