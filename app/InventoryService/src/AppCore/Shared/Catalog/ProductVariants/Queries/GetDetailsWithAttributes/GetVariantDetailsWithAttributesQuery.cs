namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithAttributes;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantDetailsWithAttributesQuery : IQuery<GetVariantDetailsWithAttributesQueryResult>
{
    public GetVariantDetailsWithAttributesQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
