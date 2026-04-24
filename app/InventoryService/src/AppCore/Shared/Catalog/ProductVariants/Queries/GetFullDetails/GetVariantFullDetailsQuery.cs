namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetFullDetails;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantFullDetailsQuery : IQuery<GetVariantFullDetailsQueryResult>
{
    public GetVariantFullDetailsQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
