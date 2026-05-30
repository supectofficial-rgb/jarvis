namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class VariantAddOn : Aggregate
{
    public Guid VariantRef { get; private set; }
    public Guid? AddOnVariantRef { get; private set; }
    public Guid? TagId { get; private set; }
    public bool IsRequired { get; private set; }

    private VariantAddOn()
    {
    }

    internal static VariantAddOn Create(Guid variantRef, Guid addOnVariantRef)
        => Create(variantRef, addOnVariantRef, null, false);

    internal static VariantAddOn Create(Guid variantRef, Guid? addOnVariantRef, Guid? tagId, bool isRequired)
        => Create(variantRef, addOnVariantRef, tagId, isRequired, Guid.NewGuid());

    internal static VariantAddOn Create(Guid variantRef, Guid? addOnVariantRef, Guid? tagId, bool isRequired, Guid businessKey)
    {
        var addOn = Create(variantRef, addOnVariantRef, tagId, isRequired);
        addOn.BusinessKey = OysterFx.AppCore.Domain.ValueObjects.BusinessKey.FromGuid(businessKey);
        return addOn;
    }

    internal void Update(Guid? addOnVariantRef, Guid? tagId, bool isRequired)
    {
        AddOnVariantRef = addOnVariantRef;
        TagId = tagId;
        IsRequired = isRequired;
    }
}
