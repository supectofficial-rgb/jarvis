namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.Common;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class CategoryVariantNameFormulaQueryRepository
    : QueryRepository<InventoryServiceQueryDbContext>, ICategoryVariantNameFormulaQueryRepository
{
    public CategoryVariantNameFormulaQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<CategoryVariantNameFormulaItem?> GetByBusinessKeyAsync(Guid formulaBusinessKey)
    {
        var items = await BuildQuery(includeInactive: true)
            .Where(x => x.Formula.BusinessKey == formulaBusinessKey)
            .ToListAsync();

        return Map(items).FirstOrDefault();
    }

    public async Task<List<CategoryVariantNameFormulaItem>> GetByCategoryIdAsync(Guid categoryRef, bool includeInactive = true)
    {
        var items = await BuildQuery(includeInactive)
            .Where(x => x.Formula.CategoryRef == categoryRef)
            .ToListAsync();

        return Map(items);
    }

    private IQueryable<FormulaPartProjection> BuildQuery(bool includeInactive)
    {
        var formulas = _dbContext.Set<CategoryVariantNameFormulaReadModel>().AsQueryable();
        if (!includeInactive)
            formulas = formulas.Where(x => x.IsActive);

        return from formula in formulas
            join part in _dbContext.Set<CategoryVariantNameFormulaPartReadModel>()
                on formula.BusinessKey equals part.FormulaRef
            join attribute in _dbContext.Set<AttributeDefinitionReadModel>()
                on part.AttributeRef equals attribute.BusinessKey
            select new FormulaPartProjection(formula, part, attribute);
    }

    private static List<CategoryVariantNameFormulaItem> Map(List<FormulaPartProjection> rows)
    {
        return rows
            .GroupBy(x => x.Formula.BusinessKey)
            .Select(group =>
            {
                var formula = group.First().Formula;
                var parts = group
                    .OrderBy(x => x.Part.SortOrder)
                    .Select(x => new CategoryVariantNameFormulaPartItem
                    {
                        PartBusinessKey = x.Part.BusinessKey,
                        AttributeRef = x.Part.AttributeRef,
                        AttributeCode = x.Attribute.Code,
                        AttributeName = x.Attribute.Name,
                        DataType = x.Attribute.DataType.ToString(),
                        Scope = x.Attribute.Scope.ToString(),
                        SortOrder = x.Part.SortOrder
                    })
                    .ToList();

                return new CategoryVariantNameFormulaItem
                {
                    FormulaBusinessKey = formula.BusinessKey,
                    CategoryRef = formula.CategoryRef,
                    Name = formula.Name,
                    Separator = formula.Separator,
                    DisplayOrder = formula.DisplayOrder,
                    IsActive = formula.IsActive,
                    Preview = string.Join(formula.Separator, parts.Select(x => x.AttributeName)),
                    Parts = parts
                };
            })
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();
    }

    private sealed record FormulaPartProjection(
        CategoryVariantNameFormulaReadModel Formula,
        CategoryVariantNameFormulaPartReadModel Part,
        AttributeDefinitionReadModel Attribute);
}
