namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetQualityStatusByIdQueryHandler : QueryHandler<GetQualityStatusByIdQuery, GetQualityStatusByBusinessKeyQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public GetQualityStatusByIdQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetQualityStatusByBusinessKeyQueryResult>> ExecuteAsync(GetQualityStatusByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.QualityStatusId);
        if (item is null)
            return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Fail("Quality status was not found.", "NOT_FOUND");

        return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Success(item);
    }
}
