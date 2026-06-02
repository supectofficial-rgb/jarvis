namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class VariantComponent : Aggregate
{
    public BusinessKey VariantRef { get; private set; } = null!;
    public Guid ComponentVariantRef { get; private set; }
    public decimal Quantity { get; private set; }

    private VariantComponent()
    {
    }

    internal static VariantComponent Create(
        BusinessKey variantRef,
        Guid componentBusinessKey,
        Guid componentVariantRef,
        decimal quantity)
    {
        if (componentBusinessKey == Guid.Empty)
            throw new ArgumentException("ComponentBusinessKey is required.", nameof(componentBusinessKey));

        return new VariantComponent
        {
            BusinessKey = componentBusinessKey,
            VariantRef = variantRef,
            ComponentVariantRef = componentVariantRef,
            Quantity = quantity
        };
    }

    internal void Update(Guid componentVariantRef, decimal quantity)
    {
        ComponentVariantRef = componentVariantRef;
        Quantity = quantity;
    }
}
