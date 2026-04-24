namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchQualityStatusesQueryHandler : QueryHandler<SearchQualityStatusesQuery, SearchQualityStatusesQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public SearchQualityStatusesQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchQualityStatusesQueryResult>> ExecuteAsync(SearchQualityStatusesQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchQualityStatusesQueryResult>.Success(result);
    }
}
