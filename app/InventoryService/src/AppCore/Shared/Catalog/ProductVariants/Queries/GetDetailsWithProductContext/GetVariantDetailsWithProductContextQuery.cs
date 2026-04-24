namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithProductContext;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantDetailsWithProductContextQuery : IQuery<GetVariantDetailsWithProductContextQueryResult>
{
    public GetVariantDetailsWithProductContextQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
