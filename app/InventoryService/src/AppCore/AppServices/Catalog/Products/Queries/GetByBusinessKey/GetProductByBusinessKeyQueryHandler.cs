namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductByBusinessKeyQueryHandler : QueryHandler<GetProductByBusinessKeyQuery, GetProductByBusinessKeyQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductByBusinessKeyQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductByBusinessKeyQueryResult>> ExecuteAsync(GetProductByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.ProductBusinessKey);
        if (item is null)
            return QueryResult<GetProductByBusinessKeyQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductByBusinessKeyQueryResult>.Success(item);
    }
}
