namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetQualityStatusByBusinessKeyQueryHandler : QueryHandler<GetQualityStatusByBusinessKeyQuery, GetQualityStatusByBusinessKeyQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public GetQualityStatusByBusinessKeyQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetQualityStatusByBusinessKeyQueryResult>> ExecuteAsync(GetQualityStatusByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.QualityStatusBusinessKey);
        if (item is null)
            return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Fail("Quality status was not found.", "NOT_FOUND");

        return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Success(item);
    }
}
