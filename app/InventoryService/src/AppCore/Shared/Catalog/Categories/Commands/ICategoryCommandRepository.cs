namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ICategoryCommandRepository : ICommandRepository<Category, long>
{
    Task<Category?> GetByBusinessKeyAsync(Guid categoryBusinessKey);
    Task<int> UpdateFieldsByBusinessKeyAsync(
        Guid categoryBusinessKey,
        string code,
        string name,
        int displayOrder,
        Guid? parentCategoryRef,
        bool isActive);
    Task DeleteGraphByBusinessKeyAsync(Guid categoryBusinessKey);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
    Task<bool> ExistsByParentRefAsync(Guid parentCategoryRef, bool onlyActive = true);
    Task<bool> ExistsRuleByAttributeRefAsync(Guid attributeRef, bool onlyActiveCategories = true, bool onlyActiveRules = true);
}
