namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionByPath;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantUomConversionByPathQuery : IQuery<GetVariantUomConversionByPathQueryResult>
{
    public GetVariantUomConversionByPathQuery(Guid variantId, Guid fromUomRef, Guid toUomRef)
    {
        VariantId = variantId;
        FromUomRef = fromUomRef;
        ToUomRef = toUomRef;
    }

    public Guid VariantId { get; }
    public Guid FromUomRef { get; }
    public Guid ToUomRef { get; }
}
