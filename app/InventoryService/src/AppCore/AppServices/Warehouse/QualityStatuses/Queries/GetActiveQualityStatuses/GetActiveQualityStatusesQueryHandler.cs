namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.GetActiveQualityStatuses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetActiveQualityStatuses;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveQualityStatusesQueryHandler : QueryHandler<GetActiveQualityStatusesQuery, GetActiveQualityStatusesQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public GetActiveQualityStatusesQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveQualityStatusesQueryResult>> ExecuteAsync(GetActiveQualityStatusesQuery request)
    {
        var items = await _repository.GetActiveQualityStatusesAsync();
        return QueryResult<GetActiveQualityStatusesQueryResult>.Success(new GetActiveQualityStatusesQueryResult
        {
            Items = items
        });
    }
}
