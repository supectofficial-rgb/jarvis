namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantComponent : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid ComponentVariantRef { get; private set; }
    public Guid WarehouseRef { get; private set; }
    public Guid LocationRef { get; private set; }
    public decimal Quantity { get; private set; }

    private VariantComponent()
    {
    }

    internal static VariantComponent Create(
        Guid variantRef,
        Guid componentBusinessKey,
        Guid componentVariantRef,
        Guid warehouseRef,
        Guid locationRef,
        decimal quantity)
    {
        if (componentBusinessKey == Guid.Empty)
            throw new ArgumentException("ComponentBusinessKey is required.", nameof(componentBusinessKey));

        return new VariantComponent
        {
            BusinessKey = componentBusinessKey,
            VariantRef = variantRef,
            ComponentVariantRef = componentVariantRef,
            WarehouseRef = warehouseRef,
            LocationRef = locationRef,
            Quantity = quantity
        };
    }

    internal void Update(Guid componentVariantRef, Guid warehouseRef, Guid locationRef, decimal quantity)
    {
        ComponentVariantRef = componentVariantRef;
        WarehouseRef = warehouseRef;
        LocationRef = locationRef;
        Quantity = quantity;
    }
}
