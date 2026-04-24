namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;
using OysterFx.AppCore.Shared.Queries;

public interface IAttributeDefinitionQueryRepository : IQueryRepository
{
    Task<GetAttributeDefinitionByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid attributeDefinitionBusinessKey);
    Task<GetAttributeDefinitionByBusinessKeyQueryResult?> GetByIdAsync(Guid attributeDefinitionId);
    Task<GetAttributeDefinitionListQueryResult> GetListAsync(GetAttributeDefinitionListQuery query);

    Task<SearchAttributeDefinitionsQueryResult> SearchAsync(SearchAttributeDefinitionsQuery query);
    Task<List<AttributeDefinitionListItem>> GetActiveAsync();
    Task<List<AttributeDefinitionListItem>> GetByScopeAsync(string scope, bool includeInactive = false);
    Task<AttributeDefinitionSummaryItem?> GetSummaryAsync(Guid attributeDefinitionId);

    Task<AttributeOptionListItem?> GetOptionByIdAsync(Guid optionId);
    Task<List<AttributeOptionListItem>> GetOptionsByAttributeIdAsync(Guid attributeDefinitionId, bool includeInactive = true);
    Task<SearchAttributeOptionsQueryResult> SearchOptionsAsync(SearchAttributeOptionsQuery query);
}
