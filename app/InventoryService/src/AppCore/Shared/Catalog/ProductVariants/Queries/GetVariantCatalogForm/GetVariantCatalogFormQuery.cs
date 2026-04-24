namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCatalogForm;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantCatalogFormQuery : IQuery<GetVariantCatalogFormQueryResult>
{
    public GetVariantCatalogFormQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
