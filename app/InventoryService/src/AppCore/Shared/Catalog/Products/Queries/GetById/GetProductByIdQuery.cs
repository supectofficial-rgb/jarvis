namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetProductByIdQuery : IQuery<GetProductByIdQueryResult>
{
    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
