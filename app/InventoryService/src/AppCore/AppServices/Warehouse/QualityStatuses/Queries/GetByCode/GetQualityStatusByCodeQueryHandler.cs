namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByCode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetQualityStatusByCodeQueryHandler : QueryHandler<GetQualityStatusByCodeQuery, GetQualityStatusByBusinessKeyQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public GetQualityStatusByCodeQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetQualityStatusByBusinessKeyQueryResult>> ExecuteAsync(GetQualityStatusByCodeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Fail("Code is required.", "VALIDATION");

        var item = await _repository.GetByCodeAsync(request.Code);
        if (item is null)
            return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Fail("Quality status was not found.", "NOT_FOUND");

        return QueryResult<GetQualityStatusByBusinessKeyQueryResult>.Success(item);
    }
}
