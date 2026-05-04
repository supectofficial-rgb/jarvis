namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantAddOnsByVariantId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantAddOnsByVariantIdQuery : IQuery<GetVariantAddOnsByVariantIdQueryResult>
{
    public GetVariantAddOnsByVariantIdQuery(Guid variantId)
    {
        VariantId = variantId;
    }

    public Guid VariantId { get; }
}
