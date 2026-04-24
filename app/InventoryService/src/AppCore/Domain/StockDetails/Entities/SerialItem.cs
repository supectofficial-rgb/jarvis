namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class SerialItem : AggregateRoot
{
    public string SerialNo { get; private set; } = string.Empty;
    public Guid VariantRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public Guid? StockDetailRef { get; private set; }
    public Guid QualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public SerialItemStatus Status { get; private set; }
    public DateTime DateScannedIn { get; private set; }
    public Guid? LastTransactionRef { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private SerialItem()
    {
    }

    private SerialItem(
        string serialNo,
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("Serial number is required.", nameof(serialNo));

        SerialNo = serialNo.Trim();
        VariantRef = variantRef;
        SellerRef = sellerRef;
        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        QualityStatusRef = qualityStatusRef;
        LotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        Status = SerialItemStatus.Available;
        DateScannedIn = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public static SerialItem Create(
        string serialNo,
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo = null)
    {
        return new SerialItem(serialNo, variantRef, sellerRef, warehouseRef, locationRef, qualityStatusRef, lotBatchNo);
    }

    public void Reserve()
    {
        EnsureStatus(SerialItemStatus.Available);
        Status = SerialItemStatus.Reserved;
        Touch();
    }

    public void ReleaseReservation()
    {
        EnsureStatus(SerialItemStatus.Reserved);
        Status = SerialItemStatus.Available;
        Touch();
    }

    public void Issue(BusinessKey transactionBusinessKey)
    {
        if (Status is not (SerialItemStatus.Available or SerialItemStatus.Reserved or SerialItemStatus.Returned))
            throw new AggregateStateExceptions("Serial item cannot be issued from its current state.", nameof(Status));

        Status = SerialItemStatus.Issued;
        LastTransactionRef = transactionBusinessKey.Value;
        Touch();
    }

    public void MarkReturned()
    {
        Status = SerialItemStatus.Returned;
        Touch();
    }

    public void ReturnToAvailable(Guid warehouseRef, Guid locationRef, Guid qualityStatusRef, string? lotBatchNo = null)
    {
        Status = SerialItemStatus.Available;
        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        QualityStatusRef = qualityStatusRef;
        LotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        Touch();
    }

    public void Move(Guid warehouseRef, Guid locationRef, Guid qualityStatusRef, string? lotBatchNo = null)
    {
        if (Status == SerialItemStatus.Issued)
            throw new AggregateStateExceptions("Issued serial item cannot be moved without return workflow.", nameof(Status));

        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        QualityStatusRef = qualityStatusRef;
        LotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        Touch();
    }

    public void Quarantine()
    {
        Status = SerialItemStatus.Quarantine;
        Touch();
    }

    public void ReleaseFromQuarantine(Guid qualityStatusRef)
    {
        EnsureStatus(SerialItemStatus.Quarantine);
        QualityStatusRef = qualityStatusRef;
        Status = SerialItemStatus.Available;
        Touch();
    }

    public void Scrap()
    {
        Status = SerialItemStatus.Scrapped;
        Touch();
    }

    public void LinkStockDetail(BusinessKey stockDetailBusinessKey) => StockDetailRef = stockDetailBusinessKey.Value;

    private void EnsureStatus(SerialItemStatus expected)
    {
        if (Status != expected)
            throw new AggregateStateExceptions($"Serial item must be in {expected} state.", nameof(Status));
    }

    private void Touch() => LastUpdatedAt = DateTime.UtcNow;
}
