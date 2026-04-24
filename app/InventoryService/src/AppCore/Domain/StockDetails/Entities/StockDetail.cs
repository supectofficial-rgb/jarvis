namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Events;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;

public sealed class StockDetail : AggregateRoot
{
    public Guid VariantRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public Guid QualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public decimal QuantityOnHand { get; private set; }
    public DateTime FirstReceivedAt { get; private set; }
    public DateTime LastReceivedAt { get; private set; }
    public DateTime? LastIssuedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private StockDetail()
    {
    }

    private StockDetail(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        decimal openingQty,
        DateTime occurredAt)
    {
        if (openingQty < 0)
            throw new ArgumentOutOfRangeException(nameof(openingQty));

        VariantRef = variantRef;
        SellerRef = sellerRef;
        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        QualityStatusRef = qualityStatusRef;
        LotBatchNo = NormalizeLot(lotBatchNo);
        QuantityOnHand = openingQty;
        FirstReceivedAt = occurredAt;
        LastReceivedAt = occurredAt;
        LastUpdatedAt = occurredAt;
        EnsureDateInvariant();
    }

    public static StockDetail Create(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        decimal openingQty,
        DateTime occurredAt)
    {
        return new StockDetail(
            variantRef,
            sellerRef,
            warehouseRef,
            locationRef,
            qualityStatusRef,
            lotBatchNo,
            openingQty,
            occurredAt);
    }

    public void Receive(decimal quantity, DateTime occurredAt)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var previous = QuantityOnHand;
        QuantityOnHand += quantity;
        LastUpdatedAt = occurredAt;

        if (FirstReceivedAt == default)
            FirstReceivedAt = occurredAt;

        LastReceivedAt = occurredAt;
        EnsureDateInvariant();
        RaiseQuantityChanged(previous, QuantityOnHand, occurredAt);
    }

    public void Issue(decimal quantity, DateTime occurredAt)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (quantity > QuantityOnHand)
            throw new AggregateStateExceptions("Issue quantity exceeds quantity on hand.", nameof(quantity));

        var previous = QuantityOnHand;
        QuantityOnHand -= quantity;
        LastIssuedAt = occurredAt;
        LastUpdatedAt = occurredAt;
        RaiseQuantityChanged(previous, QuantityOnHand, occurredAt);
    }

    public void Adjust(decimal delta, DateTime occurredAt)
    {
        var next = QuantityOnHand + delta;
        if (next < 0)
            throw new AggregateStateExceptions("Quantity on hand cannot become negative.", nameof(QuantityOnHand));

        var previous = QuantityOnHand;
        QuantityOnHand = next;
        LastUpdatedAt = occurredAt;

        if (delta > 0)
        {
            if (FirstReceivedAt == default)
                FirstReceivedAt = occurredAt;

            LastReceivedAt = occurredAt;
        }

        EnsureDateInvariant();
        RaiseQuantityChanged(previous, QuantityOnHand, occurredAt);
    }

    public void ApplyQuantity(decimal delta, DateTime occurredAt) => Adjust(delta, occurredAt);

    public void MoveTo(Guid warehouseRef, Guid locationRef, DateTime occurredAt)
    {
        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        LastUpdatedAt = occurredAt;
    }

    public void ChangeQualityStatus(Guid qualityStatusRef, DateTime occurredAt)
    {
        QualityStatusRef = qualityStatusRef;
        LastUpdatedAt = occurredAt;
    }

    public void ChangeLotBatch(string? lotBatchNo, DateTime occurredAt)
    {
        LotBatchNo = NormalizeLot(lotBatchNo);
        LastUpdatedAt = occurredAt;
    }

    private void RaiseQuantityChanged(decimal previous, decimal current, DateTime occurredAt)
    {
        AddEvent(new StockDetailQuantityChangedEvent(BusinessKey, previous, current, occurredAt));
    }

    private static string? NormalizeLot(string? lotBatchNo) => string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();

    private void EnsureDateInvariant()
    {
        if (FirstReceivedAt != default && LastReceivedAt != default && FirstReceivedAt > LastReceivedAt)
            throw new AggregateStateExceptions("FirstReceivedAt cannot be after LastReceivedAt.", nameof(FirstReceivedAt));
    }
}
