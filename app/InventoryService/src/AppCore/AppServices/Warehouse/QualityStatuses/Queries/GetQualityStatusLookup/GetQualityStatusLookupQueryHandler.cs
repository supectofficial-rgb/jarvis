namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Queries.GetQualityStatusLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetQualityStatusLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetQualityStatusLookupQueryHandler : QueryHandler<GetQualityStatusLookupQuery, GetQualityStatusLookupQueryResult>
{
    private readonly IQualityStatusQueryRepository _repository;

    public GetQualityStatusLookupQueryHandler(IQualityStatusQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetQualityStatusLookupQueryResult>> ExecuteAsync(GetQualityStatusLookupQuery request)
    {
        var items = await _repository.GetLookupAsync(request.IncludeInactive);
        return QueryResult<GetQualityStatusLookupQueryResult>.Success(new GetQualityStatusLookupQueryResult
        {
            Items = items
        });
    }
}
