namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public interface ICategoryVariantNameFormulaQueryRepository : IQueryRepository
{
    Task<CategoryVariantNameFormulaItem?> GetByBusinessKeyAsync(Guid formulaBusinessKey);
    Task<List<CategoryVariantNameFormulaItem>> GetByCategoryIdAsync(Guid categoryRef, bool includeInactive = true);
}
