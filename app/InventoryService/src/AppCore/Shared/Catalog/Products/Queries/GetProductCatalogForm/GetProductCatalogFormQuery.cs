namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCatalogForm;

using OysterFx.AppCore.Shared.Queries;

public class GetProductCatalogFormQuery : IQuery<GetProductCatalogFormQueryResult>
{
    public GetProductCatalogFormQuery(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; }
}
