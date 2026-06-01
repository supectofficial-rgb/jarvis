namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class VariantAddOn : Aggregate
{
    public BusinessKey VariantRef { get; private set; } = null!;
    public Guid? AddOnVariantRef { get; private set; }
    public Guid? TagId { get; private set; }
    public bool IsRequired { get; private set; }

    private VariantAddOn()
    {
    }

    internal static VariantAddOn Create(BusinessKey variantRef, Guid addOnVariantRef)
        => Create(variantRef, addOnVariantRef, null, false);

    internal static VariantAddOn Create(BusinessKey variantRef, Guid? addOnVariantRef, Guid? tagId, bool isRequired)
        => Create(variantRef, addOnVariantRef, tagId, isRequired, Guid.NewGuid());

    internal static VariantAddOn Create(BusinessKey variantRef, Guid? addOnVariantRef, Guid? tagId, bool isRequired, Guid businessKey)
    {
        if (businessKey == Guid.Empty)
            throw new ArgumentException("VariantAddOnBusinessKey is required.", nameof(businessKey));

        return new VariantAddOn
        {
            BusinessKey = OysterFx.AppCore.Domain.ValueObjects.BusinessKey.FromGuid(businessKey),
            VariantRef = variantRef,
            AddOnVariantRef = addOnVariantRef,
            TagId = tagId,
            IsRequired = isRequired
        };
    }

    internal void Update(Guid? addOnVariantRef, Guid? tagId, bool isRequired)
    {
        AddOnVariantRef = addOnVariantRef;
        TagId = tagId;
        IsRequired = isRequired;
    }
}
