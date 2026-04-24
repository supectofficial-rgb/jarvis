namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class FulfillmentLine : Aggregate
{
    private readonly List<FulfillmentLineSerial> _serials = new();

    public Guid FulfillmentRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public decimal Qty { get; private set; }
    public Guid UomRef { get; private set; }
    public decimal BaseQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public Guid? SourceLocationRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public Guid? ReservationAllocationRef { get; private set; }
    public FulfillmentLineStatus Status { get; private set; }
    public decimal PickedQty { get; private set; }
    public decimal PackedQty { get; private set; }
    public decimal ShippedQty { get; private set; }
    public decimal ReturnedQty { get; private set; }
    public DateTime? PickedAt { get; private set; }
    public DateTime? PackedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public IReadOnlyCollection<FulfillmentLineSerial> Serials => _serials.AsReadOnly();

    private FulfillmentLine()
    {
    }

    internal static FulfillmentLine Create(
        Guid fulfillmentRef,
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        Guid? sourceLocationRef,
        string? lotBatchNo,
        Guid? reservationAllocationRef)
    {
        if (qty <= 0 || baseQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Quantities must be greater than zero.");

        return new FulfillmentLine
        {
            FulfillmentRef = fulfillmentRef,
            VariantRef = variantRef,
            Qty = qty,
            UomRef = uomRef,
            BaseQty = baseQty,
            BaseUomRef = baseUomRef,
            SourceLocationRef = sourceLocationRef,
            LotBatchNo = lotBatchNo,
            ReservationAllocationRef = reservationAllocationRef,
            Status = FulfillmentLineStatus.Pending
        };
    }

    public void AddSerial(Guid? serialRef, string serialNo)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("Serial number is required.", nameof(serialNo));

        if (_serials.Any(x => string.Equals(x.SerialNo, serialNo, StringComparison.OrdinalIgnoreCase)))
            return;

        _serials.Add(FulfillmentLineSerial.Create(BusinessKey.Value, serialRef, serialNo.Trim()));
    }

    public void MarkPicked(decimal? quantity = null)
    {
        var nextQuantity = quantity ?? Qty;
        if (nextQuantity <= 0 || PickedQty + nextQuantity > Qty)
            throw new InvalidOperationException("Picked quantity is invalid.");

        PickedQty += nextQuantity;
        Status = PickedQty < Qty ? FulfillmentLineStatus.PartiallyPicked : FulfillmentLineStatus.Picked;
        PickedAt = DateTime.UtcNow;
    }

    public void MarkPacked(decimal? quantity = null)
    {
        if (Status is not (FulfillmentLineStatus.Picked or FulfillmentLineStatus.PartiallyPicked))
            throw new InvalidOperationException("Only picked lines can be packed.");

        var nextQuantity = quantity ?? PickedQty;
        if (nextQuantity <= 0 || PackedQty + nextQuantity > PickedQty)
            throw new InvalidOperationException("Packed quantity is invalid.");

        PackedQty += nextQuantity;
        Status = PackedQty < Qty ? FulfillmentLineStatus.PartiallyPacked : FulfillmentLineStatus.Packed;
        PackedAt = DateTime.UtcNow;
    }

    public void MarkShipped(decimal? quantity = null)
    {
        if (Status is not (FulfillmentLineStatus.Packed or FulfillmentLineStatus.PartiallyPacked))
            throw new InvalidOperationException("Only packed lines can be shipped.");

        var nextQuantity = quantity ?? PackedQty;
        if (nextQuantity <= 0 || ShippedQty + nextQuantity > PackedQty)
            throw new InvalidOperationException("Shipped quantity is invalid.");

        ShippedQty += nextQuantity;
        Status = ShippedQty < Qty ? FulfillmentLineStatus.PartiallyShipped : FulfillmentLineStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public void MarkReturned(decimal? quantity = null)
    {
        if (Status is not (FulfillmentLineStatus.Shipped or FulfillmentLineStatus.PartiallyShipped))
            throw new InvalidOperationException("Only shipped lines can be returned.");

        var nextQuantity = quantity ?? ShippedQty;
        if (nextQuantity <= 0 || ReturnedQty + nextQuantity > ShippedQty)
            throw new InvalidOperationException("Returned quantity is invalid.");

        ReturnedQty += nextQuantity;
        Status = ReturnedQty < ShippedQty ? FulfillmentLineStatus.PartiallyReturned : FulfillmentLineStatus.Returned;
        ReturnedAt = DateTime.UtcNow;
    }
}
