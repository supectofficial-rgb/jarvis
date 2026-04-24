namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class CategoryCommandRepository
    : CommandRepository<Category, InventoryServiceCommandDbContext>, ICategoryCommandRepository
{
    public CategoryCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Category?> GetByBusinessKeyAsync(Guid categoryBusinessKey)
    {
        return _dbContext.Set<Category>()
            .Include(x => x.SchemaVersions)
            .ThenInclude(x => x.Rules)
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == categoryBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalizedCode = code.Trim();

        var query = _dbContext.Set<Category>()
            .Where(x => x.Code == normalizedCode);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }

    public Task<bool> ExistsByParentRefAsync(Guid parentCategoryRef, bool onlyActive = true)
    {
        var query = _dbContext.Set<Category>().Where(x => x.ParentCategoryRef == parentCategoryRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsRuleByAttributeRefAsync(Guid attributeRef, bool onlyActiveCategories = true, bool onlyActiveRules = true)
    {
        if (attributeRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<Category>().AsQueryable();

        if (onlyActiveCategories)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync(x => x.SchemaVersions.Any(v =>
            v.IsCurrent &&
            v.Rules.Any(r => r.AttributeRef == attributeRef && (!onlyActiveRules || r.IsActive))));
    }
}
