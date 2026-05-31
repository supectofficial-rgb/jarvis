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
        var formulas = await BuildFormulaQuery(includeInactive: true)
            .Where(x => x.BusinessKey == formulaBusinessKey)
            .ToListAsync();

        return (await MapAsync(formulas)).FirstOrDefault();
    }

    public async Task<List<CategoryVariantNameFormulaItem>> GetByCategoryIdAsync(Guid categoryRef, bool includeInactive = true)
    {
        var formulas = await BuildFormulaQuery(includeInactive)
            .Where(x => x.CategoryRef == categoryRef)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return await MapAsync(formulas);
    }

    private IQueryable<CategoryVariantNameFormulaReadModel> BuildFormulaQuery(bool includeInactive)
    {
        var formulas = _dbContext.Set<CategoryVariantNameFormulaReadModel>().AsQueryable();
        if (!includeInactive)
            formulas = formulas.Where(x => x.IsActive);

        return formulas;
    }

    private async Task<List<CategoryVariantNameFormulaItem>> MapAsync(List<CategoryVariantNameFormulaReadModel> formulas)
    {
        if (formulas.Count == 0)
            return new List<CategoryVariantNameFormulaItem>();

        var formulaRefs = formulas.Select(x => x.BusinessKey).ToList();
        var parts = await (
            from part in _dbContext.Set<CategoryVariantNameFormulaPartReadModel>()
            where formulaRefs.Contains(part.FormulaRef)
            join attribute in _dbContext.Set<AttributeDefinitionReadModel>()
                on part.AttributeRef equals attribute.BusinessKey into attributes
            from attribute in attributes.DefaultIfEmpty()
            select new FormulaPartProjection(part, attribute))
            .ToListAsync();

        var partsByFormula = parts
            .GroupBy(x => x.Part.FormulaRef)
            .ToDictionary(x => x.Key, x => x.OrderBy(part => part.Part.SortOrder).ToList());

        return formulas
            .Select(formula =>
            {
                partsByFormula.TryGetValue(formula.BusinessKey, out var formulaParts);

                var partItems = (formulaParts ?? new List<FormulaPartProjection>())
                    .Select(x =>
                    {
                        var attributeName = x.Attribute?.Name ?? string.Empty;

                        return new CategoryVariantNameFormulaPartItem
                        {
                            PartBusinessKey = x.Part.BusinessKey,
                            AttributeRef = x.Part.AttributeRef,
                            Separator = x.Part.Separator,
                            AttributeCode = x.Attribute?.Code ?? string.Empty,
                            AttributeName = attributeName,
                            DataType = x.Attribute?.DataType.ToString() ?? string.Empty,
                            Scope = x.Attribute?.Scope.ToString() ?? string.Empty,
                            SortOrder = x.Part.SortOrder
                        };
                    })
                    .ToList();
                var previewParts = partItems
                    .Where(x => !string.IsNullOrWhiteSpace(x.AttributeName))
                    .ToList();

                return new CategoryVariantNameFormulaItem
                {
                    FormulaBusinessKey = formula.BusinessKey,
                    CategoryRef = formula.CategoryRef,
                    Name = formula.Name,
                    Separator = formula.Separator,
                    DisplayOrder = formula.DisplayOrder,
                    IsActive = formula.IsActive,
                    Preview = string.Concat(previewParts.Select((x, index) =>
                        index < previewParts.Count - 1
                            ? $"{x.AttributeName}{x.Separator}"
                            : x.AttributeName)),
                    Parts = partItems
                };
            })
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();
    }

    private sealed record FormulaPartProjection(
        CategoryVariantNameFormulaPartReadModel Part,
        AttributeDefinitionReadModel? Attribute);
}
