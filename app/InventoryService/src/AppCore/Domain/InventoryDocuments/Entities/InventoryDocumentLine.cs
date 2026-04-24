namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class InventoryDocumentLine : Aggregate
{
    private readonly List<InventoryDocumentLineSerial> _serials = new();

    public Guid VariantRef { get; private set; }
    public decimal Qty { get; private set; }
    public Guid UomRef { get; private set; }
    public decimal BaseQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public Guid? SourceLocationRef { get; private set; }
    public Guid? DestinationLocationRef { get; private set; }
    public Guid? QualityStatusRef { get; private set; }
    public Guid? FromQualityStatusRef { get; private set; }
    public Guid? ToQualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public string? ReasonCode { get; private set; }
    public InventoryAdjustmentDirection? AdjustmentDirection { get; private set; }
    public IReadOnlyCollection<InventoryDocumentLineSerial> Serials => _serials.AsReadOnly();

    private InventoryDocumentLine()
    {
    }

    private InventoryDocumentLine(
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        Guid? sourceLocationRef,
        Guid? destinationLocationRef,
        Guid? qualityStatusRef,
        Guid? fromQualityStatusRef,
        Guid? toQualityStatusRef,
        string? lotBatchNo,
        string? reasonCode,
        InventoryAdjustmentDirection? adjustmentDirection)
    {
        if (qty <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty), "Qty must be greater than zero.");
        if (baseQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(baseQty), "BaseQty must be greater than zero.");
        if (sourceLocationRef.HasValue && destinationLocationRef.HasValue && sourceLocationRef == destinationLocationRef)
            throw new InvalidOperationException("Source and destination locations cannot be the same.");
        if (fromQualityStatusRef.HasValue && toQualityStatusRef.HasValue && fromQualityStatusRef == toQualityStatusRef)
            throw new InvalidOperationException("From and to quality statuses cannot be the same.");

        VariantRef = variantRef;
        Qty = qty;
        UomRef = uomRef;
        BaseQty = baseQty;
        BaseUomRef = baseUomRef;
        SourceLocationRef = sourceLocationRef;
        DestinationLocationRef = destinationLocationRef;
        QualityStatusRef = qualityStatusRef;
        FromQualityStatusRef = fromQualityStatusRef;
        ToQualityStatusRef = toQualityStatusRef;
        LotBatchNo = Normalize(lotBatchNo);
        ReasonCode = Normalize(reasonCode);
        AdjustmentDirection = adjustmentDirection;
    }

    public static InventoryDocumentLine Create(
        Guid variantRef,
        decimal qty,
        Guid uomRef,
        decimal baseQty,
        Guid baseUomRef,
        Guid? sourceLocationRef = null,
        Guid? destinationLocationRef = null,
        Guid? qualityStatusRef = null,
        Guid? fromQualityStatusRef = null,
        Guid? toQualityStatusRef = null,
        string? lotBatchNo = null,
        string? reasonCode = null,
        InventoryAdjustmentDirection? adjustmentDirection = null)
    {
        return new InventoryDocumentLine(
            variantRef,
            qty,
            uomRef,
            baseQty,
            baseUomRef,
            sourceLocationRef,
            destinationLocationRef,
            qualityStatusRef,
            fromQualityStatusRef,
            toQualityStatusRef,
            lotBatchNo,
            reasonCode,
            adjustmentDirection);
    }

    public void AddSerial(Guid? serialRef, string serialNo)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("Serial number is required.", nameof(serialNo));

        if (_serials.Any(x => string.Equals(x.SerialNo, serialNo, StringComparison.OrdinalIgnoreCase)))
            return;

        _serials.Add(InventoryDocumentLineSerial.Create(BusinessKey.Value, serialRef, serialNo.Trim()));
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
