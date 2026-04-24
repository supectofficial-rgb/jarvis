namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class InventorySourceConsumption : Aggregate
{
    public Guid OutboundTransactionRef { get; private set; }
    public Guid? OutboundTransactionLineRef { get; private set; }
    public Guid SourceBalanceRef { get; private set; }
    public Guid? SourceDocumentRef { get; private set; }
    public Guid? SourceDocumentLineRef { get; private set; }
    public Guid? SourceTransactionRef { get; private set; }
    public Guid? SourceTransactionLineRef { get; private set; }
    public Guid VariantRef { get; private set; }
    public Guid SellerRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public Guid QualityStatusRef { get; private set; }
    public string? LotBatchNo { get; private set; }
    public Guid? SerialRef { get; private set; }
    public decimal ConsumedQty { get; private set; }
    public Guid BaseUomRef { get; private set; }
    public Guid? ReservationRef { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ReasonCode { get; private set; }

    private InventorySourceConsumption()
    {
    }

    internal static InventorySourceConsumption Create(
        Guid outboundTransactionRef,
        Guid? outboundTransactionLineRef,
        Guid sourceBalanceRef,
        Guid? sourceDocumentRef,
        Guid? sourceDocumentLineRef,
        Guid? sourceTransactionRef,
        Guid? sourceTransactionLineRef,
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        Guid? serialRef,
        decimal consumedQty,
        Guid baseUomRef,
        Guid? reservationRef,
        string? reasonCode)
    {
        if (consumedQty <= 0)
            throw new ArgumentOutOfRangeException(nameof(consumedQty));

        return new InventorySourceConsumption
        {
            OutboundTransactionRef = outboundTransactionRef,
            OutboundTransactionLineRef = outboundTransactionLineRef,
            SourceBalanceRef = sourceBalanceRef,
            SourceDocumentRef = sourceDocumentRef,
            SourceDocumentLineRef = sourceDocumentLineRef,
            SourceTransactionRef = sourceTransactionRef,
            SourceTransactionLineRef = sourceTransactionLineRef,
            VariantRef = variantRef,
            SellerRef = sellerRef,
            WarehouseRef = warehouseRef,
            LocationRef = locationRef,
            QualityStatusRef = qualityStatusRef,
            LotBatchNo = lotBatchNo,
            SerialRef = serialRef,
            ConsumedQty = consumedQty,
            BaseUomRef = baseUomRef,
            ReservationRef = reservationRef,
            CreatedAt = DateTime.UtcNow,
            ReasonCode = string.IsNullOrWhiteSpace(reasonCode) ? null : reasonCode.Trim()
        };
    }
}
