namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetSummary;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductSummaryQueryHandler : QueryHandler<GetProductSummaryQuery, GetProductSummaryQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductSummaryQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductSummaryQueryResult>> ExecuteAsync(GetProductSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductSummaryQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductSummaryQueryResult>.Success(new GetProductSummaryQueryResult { Item = item });
    }
}
