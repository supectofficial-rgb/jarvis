namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceBalanceByIdQueryHandler
    : QueryHandler<GetInventorySourceBalanceByIdQuery, GetInventorySourceBalanceByIdQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceBalanceByIdQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceBalanceByIdQueryResult>> ExecuteAsync(GetInventorySourceBalanceByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.SourceBalanceId);
        if (item is null)
            return QueryResult<GetInventorySourceBalanceByIdQueryResult>.Fail("Source balance was not found.", "NOT_FOUND");

        return QueryResult<GetInventorySourceBalanceByIdQueryResult>.Success(
            new GetInventorySourceBalanceByIdQueryResult { Item = item });
    }
}
