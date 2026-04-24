namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSerialItemByIdQueryHandler : QueryHandler<GetSerialItemByIdQuery, GetSerialItemByIdQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public GetSerialItemByIdQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSerialItemByIdQueryResult>> ExecuteAsync(GetSerialItemByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.SerialItemId);
        if (item is null)
            return QueryResult<GetSerialItemByIdQueryResult>.Fail("Serial item was not found.", "NOT_FOUND");

        return QueryResult<GetSerialItemByIdQueryResult>.Success(new GetSerialItemByIdQueryResult { Item = item });
    }
}
