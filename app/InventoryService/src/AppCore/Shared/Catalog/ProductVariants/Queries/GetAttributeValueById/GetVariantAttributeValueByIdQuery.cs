namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValueById;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantAttributeValueByIdQuery : IQuery<GetVariantAttributeValueByIdQueryResult>
{
    public GetVariantAttributeValueByIdQuery(Guid variantAttributeValueId){ VariantAttributeValueId = variantAttributeValueId; } public Guid VariantAttributeValueId { get; }
}
