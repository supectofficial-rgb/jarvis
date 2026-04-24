namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetProductSummaryQuery : IQuery<GetProductSummaryQueryResult>
{
    public GetProductSummaryQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
