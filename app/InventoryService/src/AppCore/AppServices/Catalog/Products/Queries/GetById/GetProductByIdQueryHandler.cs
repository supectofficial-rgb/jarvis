namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductByIdQueryHandler : QueryHandler<GetProductByIdQuery, GetProductByIdQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductByIdQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductByIdQueryResult>> ExecuteAsync(GetProductByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductByIdQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductByIdQueryResult>.Success(new GetProductByIdQueryResult { Item = item });
    }
}
