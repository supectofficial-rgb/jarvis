namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionsByVariantId;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantUomConversionsByVariantIdQuery : IQuery<GetVariantUomConversionsByVariantIdQueryResult>
{
    public GetVariantUomConversionsByVariantIdQuery(Guid variantId)
    {
        VariantId = variantId;
    }

    public Guid VariantId { get; }
}
