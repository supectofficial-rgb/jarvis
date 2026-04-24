namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.GetAvailableSerialItems;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetAvailableSerialItems;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAvailableSerialItemsQueryHandler : QueryHandler<GetAvailableSerialItemsQuery, GetAvailableSerialItemsQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public GetAvailableSerialItemsQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAvailableSerialItemsQueryResult>> ExecuteAsync(GetAvailableSerialItemsQuery request)
    {
        var items = await _repository.GetAvailableAsync(request.VariantRef, request.WarehouseRef);
        return QueryResult<GetAvailableSerialItemsQueryResult>.Success(new GetAvailableSerialItemsQueryResult { Items = items });
    }
}
