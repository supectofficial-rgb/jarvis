namespace Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class InventoryTransactionLine : Aggregate
{
    public Guid? StockDetailRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public decimal InputQty { get; private set; }
    public Guid InputUomRef { get; private set; }
    public decimal BaseQtyDelta { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public Guid? SourceLocationRef { get; private set; }
    public Guid? DestinationLocationRef { get; private set; }
    public Guid? OldQualityStatusRef { get; private set; }
    public Guid? NewQualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public Guid? SerialRef { get; private set; }
    public string? ReasonCode { get; private set; }

    private InventoryTransactionLine()
    {
    }

    private InventoryTransactionLine(
        Guid variantRef,
        decimal inputQty,
        Guid inputUomRef,
        decimal baseQtyDelta,
        Guid baseUomRef,
        Guid? sourceLocationRef,
        Guid? destinationLocationRef,
        Guid? oldQualityStatusRef,
        Guid? newQualityStatusRef,
        string? lotBatchNo,
        Guid? serialRef,
        string? reasonCode)
    {
        if (inputQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(inputQty), "Input quantity must be greater than zero.");

        if (baseQtyDelta == 0)
            throw new ArgumentOutOfRangeException(nameof(baseQtyDelta), "Base quantity delta cannot be zero.");

        if (sourceLocationRef.HasValue && destinationLocationRef.HasValue && sourceLocationRef == destinationLocationRef)
            throw new InvalidOperationException("Source and destination locations cannot be the same.");

        VariantRef = variantRef;
        InputQty = inputQty;
        InputUomRef = inputUomRef;
        BaseQtyDelta = baseQtyDelta;
        BaseUomRef = baseUomRef;
        SourceLocationRef = sourceLocationRef;
        DestinationLocationRef = destinationLocationRef;
        OldQualityStatusRef = oldQualityStatusRef;
        NewQualityStatusRef = newQualityStatusRef;
        LotBatchNo = Normalize(lotBatchNo);
        SerialRef = serialRef;
        ReasonCode = Normalize(reasonCode);
    }

    public static InventoryTransactionLine Create(
        Guid variantRef,
        decimal inputQty,
        Guid inputUomRef,
        decimal baseQtyDelta,
        Guid baseUomRef,
        Guid? sourceLocationRef = null,
        Guid? destinationLocationRef = null,
        Guid? oldQualityStatusRef = null,
        Guid? newQualityStatusRef = null,
        string? lotBatchNo = null,
        Guid? serialRef = null,
        string? reasonCode = null)
    {
        return new InventoryTransactionLine(
            variantRef,
            inputQty,
            inputUomRef,
            baseQtyDelta,
            baseUomRef,
            sourceLocationRef,
            destinationLocationRef,
            oldQualityStatusRef,
            newQualityStatusRef,
            lotBatchNo,
            serialRef,
            reasonCode);
    }

    public void LinkStockDetail(BusinessKey stockDetailBusinessKey)
    {
        StockDetailRef = stockDetailBusinessKey.Value;
    }

    public void SetSerial(Guid serialRef)
    {
        SerialRef = serialRef;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
