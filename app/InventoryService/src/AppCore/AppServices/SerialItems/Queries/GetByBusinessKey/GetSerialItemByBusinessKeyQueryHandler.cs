namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSerialItemByBusinessKeyQueryHandler : QueryHandler<GetSerialItemByBusinessKeyQuery, GetSerialItemByBusinessKeyQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public GetSerialItemByBusinessKeyQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSerialItemByBusinessKeyQueryResult>> ExecuteAsync(GetSerialItemByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.SerialItemBusinessKey);
        if (item is null)
            return QueryResult<GetSerialItemByBusinessKeyQueryResult>.Fail("Serial item was not found.", "NOT_FOUND");

        return QueryResult<GetSerialItemByBusinessKeyQueryResult>.Success(item);
    }
}
