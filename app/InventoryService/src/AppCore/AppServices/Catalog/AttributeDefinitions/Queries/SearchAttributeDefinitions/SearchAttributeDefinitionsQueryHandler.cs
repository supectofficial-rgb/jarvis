namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchAttributeDefinitionsQueryHandler : QueryHandler<SearchAttributeDefinitionsQuery, SearchAttributeDefinitionsQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public SearchAttributeDefinitionsQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchAttributeDefinitionsQueryResult>> ExecuteAsync(SearchAttributeDefinitionsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchAttributeDefinitionsQueryResult>.Success(result);
    }
}
