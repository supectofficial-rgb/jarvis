namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetProductCatalogForm;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCatalogForm;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductCatalogFormQueryHandler : QueryHandler<GetProductCatalogFormQuery, GetProductCatalogFormQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductCatalogFormQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductCatalogFormQueryResult>> ExecuteAsync(GetProductCatalogFormQuery request)
    {
        var item = await _repository.GetCatalogFormAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductCatalogFormQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductCatalogFormQueryResult>.Success(new GetProductCatalogFormQueryResult { Item = item });
    }
}
