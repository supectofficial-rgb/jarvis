namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Queries.GetCategorySummary;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategorySummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategorySummaryQueryHandler : QueryHandler<GetCategorySummaryQuery, GetCategorySummaryQueryResult>
{
    private readonly ICategoryQueryRepository _repository;

    public GetCategorySummaryQueryHandler(ICategoryQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategorySummaryQueryResult>> ExecuteAsync(GetCategorySummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.CategoryId);
        if (item is null)
            return QueryResult<GetCategorySummaryQueryResult>.Fail("Category was not found.", "NOT_FOUND");

        return QueryResult<GetCategorySummaryQueryResult>.Success(new GetCategorySummaryQueryResult { Item = item });
    }
}
