namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ICategoryVariantNameFormulaCommandRepository : ICommandRepository<CategoryVariantNameFormula, long>
{
    Task<CategoryVariantNameFormula?> GetByBusinessKeyAsync(Guid formulaBusinessKey);
    Task<bool> ExistsByCategoryAndNameAsync(Guid categoryRef, string name, Guid? exceptBusinessKey = null);
    Task<int> DeleteByBusinessKeyAsync(Guid formulaBusinessKey);
}
