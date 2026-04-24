namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetDetailsWithAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithAttributes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductDetailsWithAttributesQueryHandler : QueryHandler<GetProductDetailsWithAttributesQuery, GetProductDetailsWithAttributesQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductDetailsWithAttributesQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductDetailsWithAttributesQueryResult>> ExecuteAsync(GetProductDetailsWithAttributesQuery request)
    {
        var item = await _repository.GetDetailsWithAttributesAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductDetailsWithAttributesQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductDetailsWithAttributesQueryResult>.Success(new GetProductDetailsWithAttributesQueryResult { Item = item });
    }
}
