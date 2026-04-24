namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByProductId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantsByProductIdQuery : IQuery<GetVariantsByProductIdQueryResult>
{
    public GetVariantsByProductIdQuery(Guid productId, bool includeInactive = false){ ProductId = productId; IncludeInactive = includeInactive; } public Guid ProductId { get; } public bool IncludeInactive { get; }
}
