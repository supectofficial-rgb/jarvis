namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetFullDetails;

using OysterFx.AppCore.Shared.Queries;

public class GetProductFullDetailsQuery : IQuery<GetProductFullDetailsQueryResult>
{
    public GetProductFullDetailsQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
