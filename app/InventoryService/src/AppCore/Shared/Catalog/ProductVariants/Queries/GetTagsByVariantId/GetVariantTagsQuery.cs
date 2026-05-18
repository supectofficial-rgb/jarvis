namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagsByVariantId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantTagsQuery : IQuery<GetVariantTagsQueryResult>
{
    public GetVariantTagsQuery(Guid variantId)
    {
        VariantId = variantId;
    }

    public Guid VariantId { get; }
}
