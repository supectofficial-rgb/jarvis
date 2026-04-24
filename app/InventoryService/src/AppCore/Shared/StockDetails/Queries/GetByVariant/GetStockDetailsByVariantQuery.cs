namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByVariant;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsByVariantQuery : IQuery<GetStockDetailsByVariantQueryResult>
{
    public GetStockDetailsByVariantQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}
