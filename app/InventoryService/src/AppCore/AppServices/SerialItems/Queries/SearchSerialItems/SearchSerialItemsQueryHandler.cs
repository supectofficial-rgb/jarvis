namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Queries.SearchSerialItems;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchSerialItemsQueryHandler : QueryHandler<SearchSerialItemsQuery, SearchSerialItemsQueryResult>
{
    private readonly ISerialItemQueryRepository _repository;

    public SearchSerialItemsQueryHandler(ISerialItemQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchSerialItemsQueryResult>> ExecuteAsync(SearchSerialItemsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchSerialItemsQueryResult>.Success(result);
    }
}
