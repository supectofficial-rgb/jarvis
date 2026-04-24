namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetMissingRequiredAttributes;

using OysterFx.AppCore.Shared.Queries;

public class GetMissingRequiredVariantAttributesQuery : IQuery<GetMissingRequiredVariantAttributesQueryResult>
{
    public GetMissingRequiredVariantAttributesQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
