namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByScope;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeDefinitionsByScopeQuery : IQuery<GetAttributeDefinitionsByScopeQueryResult>
{
    public GetAttributeDefinitionsByScopeQuery(string scope, bool includeInactive = false)
    {
        Scope = scope;
        IncludeInactive = includeInactive;
    }

    public string Scope { get; }
    public bool IncludeInactive { get; }
}
