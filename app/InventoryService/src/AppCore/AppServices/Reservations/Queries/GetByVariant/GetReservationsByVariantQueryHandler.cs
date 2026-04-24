namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByVariant;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationsByVariantQueryHandler
    : QueryHandler<GetReservationsByVariantQuery, GetReservationsByVariantQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationsByVariantQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationsByVariantQueryResult>> ExecuteAsync(GetReservationsByVariantQuery request)
    {
        var items = await _repository.GetByVariantAsync(request.VariantRef);
        return QueryResult<GetReservationsByVariantQueryResult>.Success(new GetReservationsByVariantQueryResult { Items = items });
    }
}
