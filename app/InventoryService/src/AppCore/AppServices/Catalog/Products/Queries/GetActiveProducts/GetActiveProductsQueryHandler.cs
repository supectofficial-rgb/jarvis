namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetActiveProducts;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetActiveProducts;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveProductsQueryHandler : QueryHandler<GetActiveProductsQuery, GetActiveProductsQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetActiveProductsQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveProductsQueryResult>> ExecuteAsync(GetActiveProductsQuery request)
    {
        var items = await _repository.GetActiveAsync();
        return QueryResult<GetActiveProductsQueryResult>.Success(new GetActiveProductsQueryResult { Items = items });
    }
}
