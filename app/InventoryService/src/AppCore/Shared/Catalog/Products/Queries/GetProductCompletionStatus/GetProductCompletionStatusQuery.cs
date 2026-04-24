namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCompletionStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetProductCompletionStatusQuery : IQuery<GetProductCompletionStatusQueryResult>
{
    public GetProductCompletionStatusQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
