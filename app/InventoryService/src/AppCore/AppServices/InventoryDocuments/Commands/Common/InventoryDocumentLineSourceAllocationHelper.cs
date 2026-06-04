namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using OysterFx.AppCore.Domain.Exceptions;

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
        IInventorySourceBalanceCommandRepository repository)
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

        var sourceBalances = await repository.GetOpenByPoolAsync(
            line.VariantRef,
            document.WarehouseRef,
            line.QualityStatusRef,
            line.LotBatchNo);

        sourceBalances = sourceBalances
            .Where(x => x.LocationRef == line.SourceLocationRef.Value)
            .OrderBy(x => x.OpenedAt)
            .ThenBy(x => x.Id)
            .ToList();

        if (sourceBalances.Count == 0)
        {
            throw new AggregateStateExceptions("No open source balance found for the selected line.", nameof(line.VariantRef));
        }

        var remaining = line.BaseQty;
        foreach (var sourceBalance in sourceBalances)
        {
            if (remaining <= 0)
            {
                break;
            }

            var quantityToAllocate = Math.Min(sourceBalance.AvailableQty, remaining);
            if (quantityToAllocate <= 0)
            {
                continue;
            }

            sourceBalance.Allocate(line.BusinessKey.Value, sourceBalance.BusinessKey.Value, quantityToAllocate);
            remaining -= quantityToAllocate;
        }

        if (remaining > 0)
        {
            throw new AggregateStateExceptions(
                $"document line '{line.BusinessKey.Value:D}' does not have enough open source balance to cover {line.BaseQty} base units.",
                nameof(line.BaseQty));
        }
    }
}
