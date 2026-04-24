namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetActiveAttributeDefinitions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeDefinitions;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveAttributeDefinitionsQueryHandler : QueryHandler<GetActiveAttributeDefinitionsQuery, GetActiveAttributeDefinitionsQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetActiveAttributeDefinitionsQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveAttributeDefinitionsQueryResult>> ExecuteAsync(GetActiveAttributeDefinitionsQuery request)
    {
        var items = await _repository.GetActiveAsync();
        return QueryResult<GetActiveAttributeDefinitionsQueryResult>.Success(new GetActiveAttributeDefinitionsQueryResult { Items = items });
    }
}
