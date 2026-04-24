namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByVariant;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSerialItemsByVariantQueryHandler : QueryHandler<GetSerialItemsByVariantQuery, GetSerialItemsByVariantQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public GetSerialItemsByVariantQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSerialItemsByVariantQueryResult>> ExecuteAsync(GetSerialItemsByVariantQuery request)
    {
        var items = await _repository.GetByVariantAsync(request.VariantRef);
        return QueryResult<GetSerialItemsByVariantQueryResult>.Success(new GetSerialItemsByVariantQueryResult { Items = items });
    }
}
