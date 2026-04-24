namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantByIdQuery : IQuery<GetVariantByIdQueryResult>
{
    public GetVariantByIdQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}
