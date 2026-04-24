namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantUomConversion : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid FromUomRef { get; private set; }
    public Guid ToUomRef { get; private set; }
    public decimal Factor { get; private set; }
    public UomRoundingMode RoundingMode { get; private set; }
    public bool IsBasePath { get; private set; }

    private VariantUomConversion()
    {
    }

    internal static VariantUomConversion Create(
        Guid variantRef,
        Guid fromUomRef,
        Guid toUomRef,
        decimal factor,
        UomRoundingMode roundingMode,
        bool isBasePath)
    {
        return new VariantUomConversion
        {
            VariantRef = variantRef,
            FromUomRef = fromUomRef,
            ToUomRef = toUomRef,
            Factor = factor,
            RoundingMode = roundingMode,
            IsBasePath = isBasePath
        };
    }

    internal void Update(decimal factor, UomRoundingMode roundingMode, bool isBasePath)
    {
        Factor = factor;
        RoundingMode = roundingMode;
        IsBasePath = isBasePath;
    }
}
