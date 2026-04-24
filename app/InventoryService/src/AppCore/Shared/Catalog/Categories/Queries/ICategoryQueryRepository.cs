namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;
using OysterFx.AppCore.Shared.Queries;

public interface ICategoryQueryRepository : IQueryRepository
{
    Task<GetCategoryByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid categoryBusinessKey);
    Task<GetCategoryByBusinessKeyQueryResult?> GetByIdAsync(Guid categoryId);
    Task<GetCategoryAttributesQueryResult?> GetAttributesAsync(Guid categoryBusinessKey, bool includeInherited = true, bool includeInactive = false);

    Task<List<CategoryTreeItem>> GetTreeAsync(bool includeInactive = false);
    Task<List<CategoryListItem>> GetRootCategoriesAsync(bool includeInactive = false);
    Task<List<CategoryListItem>> GetChildCategoriesAsync(Guid parentCategoryId, bool includeInactive = false);
    Task<List<CategoryBreadcrumbItem>> GetBreadcrumbAsync(Guid categoryId);
    Task<SearchCategoriesQueryResult> SearchAsync(SearchCategoriesQuery query);
    Task<List<CategoryListItem>> GetActiveCategoriesAsync();
    Task<CategorySummaryItem?> GetSummaryAsync(Guid categoryId);

    Task<CategoryAttributeRuleViewItem?> GetCategoryAttributeRuleByIdAsync(Guid categoryAttributeRuleId);
    Task<List<CategoryAttributeRuleViewItem>> GetCategoryAttributeRulesByCategoryIdAsync(Guid categoryId, bool includeInherited = true, bool includeInactive = false);
}
