namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantAddOn : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid AddOnVariantRef { get; private set; }

    private VariantAddOn()
    {
    }

    internal static VariantAddOn Create(Guid variantRef, Guid addOnVariantRef)
    {
        return new VariantAddOn
        {
            VariantRef = variantRef,
            AddOnVariantRef = addOnVariantRef
        };
    }
}
