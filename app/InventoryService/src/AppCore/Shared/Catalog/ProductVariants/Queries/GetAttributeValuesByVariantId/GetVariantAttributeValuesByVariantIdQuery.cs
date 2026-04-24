namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesByVariantId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantAttributeValuesByVariantIdQuery : IQuery<GetVariantAttributeValuesByVariantIdQueryResult>
{
    public GetVariantAttributeValuesByVariantIdQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
