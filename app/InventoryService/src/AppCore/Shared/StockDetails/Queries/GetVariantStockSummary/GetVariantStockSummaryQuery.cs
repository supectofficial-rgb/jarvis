namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetVariantStockSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantStockSummaryQuery : IQuery<GetVariantStockSummaryQueryResult>
{
    public GetVariantStockSummaryQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}
