namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetProductByBusinessKeyQuery : IQuery<GetProductByBusinessKeyQueryResult>
{
    public GetProductByBusinessKeyQuery(Guid productBusinessKey)
    {
        ProductBusinessKey = productBusinessKey;
    }

    public Guid ProductBusinessKey { get; }
}
