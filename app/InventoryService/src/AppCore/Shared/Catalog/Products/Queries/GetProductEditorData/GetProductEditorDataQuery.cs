namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductEditorData;

using OysterFx.AppCore.Shared.Queries;

public class GetProductEditorDataQuery : IQuery<GetProductEditorDataQueryResult>
{
    public GetProductEditorDataQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
