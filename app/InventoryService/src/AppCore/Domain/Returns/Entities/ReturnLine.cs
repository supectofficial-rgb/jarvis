namespace Insurance.InventoryService.AppCore.Domain.Returns.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class ReturnLine : Aggregate
{
    private readonly List<ReturnLineSerial> _serials = new();

    public Guid ReturnRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public decimal Qty { get; private set; }
    public Guid UomRef { get; private set; }
    public decimal BaseQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public string? ExpectedCondition { get; private set; }
    public string? ReceivedCondition { get; private set; }
    public ReturnDisposition Disposition { get; private set; }
    public decimal ReceivedQty { get; private set; }
    public ReturnLineStatus Status { get; private set; }
    public IReadOnlyCollection<ReturnLineSerial> Serials => _serials.AsReadOnly();

    private ReturnLine()
    {
    }

    internal static ReturnLine Create(
        Guid returnRef,
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        string? lotBatchNo,
        string? expectedCondition,
        ReturnDisposition disposition)
    {
        if (qty <= 0 || baseQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Quantities must be greater than zero.");

        return new ReturnLine
        {
            ReturnRef = returnRef,
            VariantRef = variantRef,
            Qty = qty,
            UomRef = uomRef,
            BaseQty = baseQty,
            BaseUomRef = baseUomRef,
            LotBatchNo = lotBatchNo,
            ExpectedCondition = expectedCondition,
            Disposition = disposition,
            Status = ReturnLineStatus.Pending
        };
    }

    public void SetReceivedCondition(string? receivedCondition) => ReceivedCondition = string.IsNullOrWhiteSpace(receivedCondition) ? null : receivedCondition.Trim();

    public void ChangeDisposition(ReturnDisposition disposition) => Disposition = disposition;

    public void AddSerial(Guid? serialRef, string serialNo)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("Serial number is required.", nameof(serialNo));

        if (_serials.Any(x => string.Equals(x.SerialNo, serialNo, StringComparison.OrdinalIgnoreCase)))
            return;

        _serials.Add(ReturnLineSerial.Create(BusinessKey.Value, serialRef, serialNo.Trim()));
    }

    public void Receive(decimal quantity, string? receivedCondition = null)
    {
        if (quantity <= 0 || ReceivedQty + quantity > Qty)
            throw new InvalidOperationException("Received quantity is invalid.");

        ReceivedQty += quantity;
        SetReceivedCondition(receivedCondition);
        Status = ReceivedQty < Qty ? ReturnLineStatus.PartiallyReceived : ReturnLineStatus.Received;
    }

    public void Close()
    {
        if (ReceivedQty <= 0)
            throw new InvalidOperationException("Return line must be received before closing.");

        Status = ReturnLineStatus.Closed;
    }
}
