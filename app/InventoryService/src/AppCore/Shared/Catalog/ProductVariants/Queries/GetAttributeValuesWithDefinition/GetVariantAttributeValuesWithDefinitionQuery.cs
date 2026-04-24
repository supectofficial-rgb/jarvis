namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesWithDefinition;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantAttributeValuesWithDefinitionQuery : IQuery<GetVariantAttributeValuesWithDefinitionQueryResult>
{
    public GetVariantAttributeValuesWithDefinitionQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
