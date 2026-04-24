namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.GetBySerialNo;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetBySerialNo;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSerialItemBySerialNoQueryHandler : QueryHandler<GetSerialItemBySerialNoQuery, GetSerialItemBySerialNoQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public GetSerialItemBySerialNoQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSerialItemBySerialNoQueryResult>> ExecuteAsync(GetSerialItemBySerialNoQuery request)
    {
        var item = await _repository.GetBySerialNoAsync(request.SerialNo, request.VariantRef);
        if (item is null)
            return QueryResult<GetSerialItemBySerialNoQueryResult>.Fail("Serial item was not found.", "NOT_FOUND");

        return QueryResult<GetSerialItemBySerialNoQueryResult>.Success(new GetSerialItemBySerialNoQueryResult { Item = item });
    }
}
