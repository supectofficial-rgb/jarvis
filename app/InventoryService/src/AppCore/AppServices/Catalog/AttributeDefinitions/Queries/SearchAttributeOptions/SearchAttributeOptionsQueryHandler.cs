namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchAttributeOptionsQueryHandler : QueryHandler<SearchAttributeOptionsQuery, SearchAttributeOptionsQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public SearchAttributeOptionsQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchAttributeOptionsQueryResult>> ExecuteAsync(SearchAttributeOptionsQuery request)
    {
        var result = await _repository.SearchOptionsAsync(request);
        return QueryResult<SearchAttributeOptionsQueryResult>.Success(result);
    }
}
