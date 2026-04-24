namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetByScope;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByScope;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeDefinitionsByScopeQueryHandler : QueryHandler<GetAttributeDefinitionsByScopeQuery, GetAttributeDefinitionsByScopeQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeDefinitionsByScopeQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeDefinitionsByScopeQueryResult>> ExecuteAsync(GetAttributeDefinitionsByScopeQuery request)
    {
        var items = await _repository.GetByScopeAsync(request.Scope, request.IncludeInactive);
        return QueryResult<GetAttributeDefinitionsByScopeQueryResult>.Success(new GetAttributeDefinitionsByScopeQueryResult { Items = items });
    }
}
