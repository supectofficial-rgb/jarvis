namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using OysterFx.AppCore.Domain.Exceptions;

internal static class InventoryDocumentLineSerialStatusHelper
{
    public static bool ShouldReserveSerials(InventoryDocumentType documentType)
        => documentType is InventoryDocumentType.Issue
            or InventoryDocumentType.Transfer
            or InventoryDocumentType.ReturnFromBuy
            or InventoryDocumentType.ReturnFromTransfer;

    public static async Task<IReadOnlyList<SerialItem>> ResolveSerialItemsAsync(
        IEnumerable<(Guid? SerialRef, string SerialNo)> serials,
        Guid variantRef,
        ISerialItemCommandRepository commandRepository,
        ISerialItemQueryRepository queryRepository)
    {
        var resolvedSerialItems = new List<SerialItem>();
        var resolvedSerialItemBusinessKeys = new HashSet<Guid>();
        var resolvedSerialNos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var serial in serials ?? Array.Empty<(Guid? SerialRef, string SerialNo)>())
        {
            SerialItem? serialItem = null;

            if (serial.SerialRef.HasValue && serial.SerialRef.Value != Guid.Empty)
            {
                serialItem = await commandRepository.GetByBusinessKeyAsync(serial.SerialRef.Value);
            }
            else if (!string.IsNullOrWhiteSpace(serial.SerialNo))
            {
                var normalizedSerialNo = serial.SerialNo.Trim();
                var serialLookup = await queryRepository.GetBySerialNoAsync(normalizedSerialNo, variantRef);
                if (serialLookup is not null)
                {
                    serialItem = await commandRepository.GetByBusinessKeyAsync(serialLookup.SerialItemBusinessKey);
                }
            }
            else
            {
                continue;
            }

            if (serialItem is null)
            {
                throw new AggregateStateExceptions($"Serial item '{serial.SerialNo}' was not found.", nameof(serial.SerialNo));
            }

            if (!resolvedSerialItemBusinessKeys.Add(serialItem.BusinessKey.Value))
            {
                continue;
            }

            resolvedSerialNos.Add(serialItem.SerialNo);
            resolvedSerialItems.Add(serialItem);
        }

        return resolvedSerialItems;
    }

    public static void ReserveSerialItems(IEnumerable<SerialItem> serialItems)
    {
        foreach (var serialItem in serialItems)
        {
            switch (serialItem.Status)
            {
                case SerialItemStatus.Available:
                    serialItem.Reserve();
                    break;
                case SerialItemStatus.Reserved:
                    throw new AggregateStateExceptions("Serial item is already reserved.", nameof(serialItem.Status));
                default:
                    throw new AggregateStateExceptions("Serial item cannot be reserved from its current state.", nameof(serialItem.Status));
            }
        }
    }

    public static void ReleaseSerialItems(IEnumerable<SerialItem> serialItems)
    {
        foreach (var serialItem in serialItems)
        {
            switch (serialItem.Status)
            {
                case SerialItemStatus.Reserved:
                    serialItem.ReleaseReservation();
                    break;
                case SerialItemStatus.Available:
                    break;
                default:
                    throw new AggregateStateExceptions("Serial item cannot be released from its current state.", nameof(serialItem.Status));
            }
        }
    }
}
