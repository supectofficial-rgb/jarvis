namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetFullDetails;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetFullDetails;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductFullDetailsQueryHandler : QueryHandler<GetProductFullDetailsQuery, GetProductFullDetailsQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductFullDetailsQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductFullDetailsQueryResult>> ExecuteAsync(GetProductFullDetailsQuery request)
    {
        var item = await _repository.GetFullDetailsAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductFullDetailsQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductFullDetailsQueryResult>.Success(new GetProductFullDetailsQueryResult { Item = item });
    }
}
