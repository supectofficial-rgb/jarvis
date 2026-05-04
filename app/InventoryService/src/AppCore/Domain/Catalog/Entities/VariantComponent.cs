namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantComponent : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid ComponentVariantRef { get; private set; }
    public decimal Quantity { get; private set; }

    private VariantComponent()
    {
    }

    internal static VariantComponent Create(Guid variantRef, Guid componentVariantRef, decimal quantity)
    {
        return new VariantComponent
        {
            VariantRef = variantRef,
            ComponentVariantRef = componentVariantRef,
            Quantity = quantity
        };
    }

    internal void Update(decimal quantity)
    {
        Quantity = quantity;
    }
}
