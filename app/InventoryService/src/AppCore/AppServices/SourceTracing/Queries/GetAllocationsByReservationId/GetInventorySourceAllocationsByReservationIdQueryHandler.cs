namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetAllocationsByReservationId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsByReservationId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceAllocationsByReservationIdQueryHandler
    : QueryHandler<GetInventorySourceAllocationsByReservationIdQuery, GetInventorySourceAllocationsByReservationIdQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceAllocationsByReservationIdQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceAllocationsByReservationIdQueryResult>> ExecuteAsync(GetInventorySourceAllocationsByReservationIdQuery request)
    {
        var items = await _repository.GetAllocationsByReservationIdAsync(request.ReservationRef);
        return QueryResult<GetInventorySourceAllocationsByReservationIdQueryResult>.Success(
            new GetInventorySourceAllocationsByReservationIdQueryResult { Items = items });
    }
}
