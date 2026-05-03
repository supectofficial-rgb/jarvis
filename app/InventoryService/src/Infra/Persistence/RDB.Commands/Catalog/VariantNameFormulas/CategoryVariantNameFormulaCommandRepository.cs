namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.VariantNameFormulas;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class CategoryVariantNameFormulaCommandRepository
    : CommandRepository<CategoryVariantNameFormula, InventoryServiceCommandDbContext>, ICategoryVariantNameFormulaCommandRepository
{
    public CategoryVariantNameFormulaCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<CategoryVariantNameFormula?> GetByBusinessKeyAsync(Guid formulaBusinessKey)
    {
        var businessKey = BusinessKey.FromGuid(formulaBusinessKey);
        return _dbContext.Set<CategoryVariantNameFormula>()
            .Include(x => x.Parts)
            .FirstOrDefaultAsync(x => x.BusinessKey == businessKey);
    }

    public Task<bool> ExistsByCategoryAndNameAsync(Guid categoryRef, string name, Guid? exceptBusinessKey = null)
    {
        var normalized = name.Trim();
        var query = _dbContext.Set<CategoryVariantNameFormula>()
            .Where(x => x.CategoryRef == categoryRef && x.Name == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }

    public async Task<int> DeleteByBusinessKeyAsync(Guid formulaBusinessKey)
    {
        var key = BusinessKey.FromGuid(formulaBusinessKey);
        await _dbContext.Set<CategoryVariantNameFormulaPart>()
            .Where(x => x.FormulaRef == formulaBusinessKey)
            .ExecuteDeleteAsync();

        return await _dbContext.Set<CategoryVariantNameFormula>()
            .Where(x => x.BusinessKey == key)
            .ExecuteDeleteAsync();
    }
}
