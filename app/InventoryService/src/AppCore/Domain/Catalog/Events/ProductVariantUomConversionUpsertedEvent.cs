namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record ProductVariantUomConversionUpsertedEvent : IDomainEvent
{
    public BusinessKey ProductVariantBusinessKey { get; }
    public Guid FromUomRef { get; }
    public Guid ToUomRef { get; }
    public decimal Factor { get; }
    public UomRoundingMode RoundingMode { get; }
    public bool IsBasePath { get; }
    public DateTime OccurredOn { get; }

    public ProductVariantUomConversionUpsertedEvent(
        BusinessKey productVariantBusinessKey,
        Guid fromUomRef,
        Guid toUomRef,
        decimal factor,
        UomRoundingMode roundingMode,
        bool isBasePath)
    {
        ProductVariantBusinessKey = productVariantBusinessKey;
        FromUomRef = fromUomRef;
        ToUomRef = toUomRef;
        Factor = factor;
        RoundingMode = roundingMode;
        IsBasePath = isBasePath;
        OccurredOn = DateTime.UtcNow;
    }
}
