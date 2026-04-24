namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductsByCategoryIdQueryHandler : QueryHandler<GetProductsByCategoryIdQuery, GetProductsByCategoryIdQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductsByCategoryIdQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductsByCategoryIdQueryResult>> ExecuteAsync(GetProductsByCategoryIdQuery request)
    {
        var items = await _repository.GetByCategoryIdAsync(request.CategoryId, request.IncludeInactive);
        return QueryResult<GetProductsByCategoryIdQueryResult>.Success(new GetProductsByCategoryIdQueryResult { Items = items });
    }
}
