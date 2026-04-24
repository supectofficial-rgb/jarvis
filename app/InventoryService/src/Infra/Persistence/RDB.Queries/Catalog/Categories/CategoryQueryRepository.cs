namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.SearchCategories;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class CategoryQueryRepository
    : QueryRepository<InventoryServiceQueryDbContext>, ICategoryQueryRepository
{
    public CategoryQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetCategoryByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid categoryBusinessKey)
    {
        var aggregate = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == categoryBusinessKey);

        if (aggregate is null)
            return null;

        var currentSchemaVersionRef = await _dbContext.Set<CategorySchemaVersionReadModel>()
            .Where(x => x.CategoryRef == categoryBusinessKey && x.IsCurrent)
            .OrderByDescending(x => x.VersionNo)
            .Select(x => (Guid?)x.BusinessKey)
            .FirstOrDefaultAsync();

        var rules = currentSchemaVersionRef.HasValue
            ? await _dbContext.Set<CategoryAttributeRuleReadModel>()
                .Where(x => x.CategorySchemaVersionRef == currentSchemaVersionRef.Value)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new CategoryAttributeRuleResultItem
                {
                    CategorySchemaVersionRef = x.CategorySchemaVersionRef,
                    AttributeRef = x.AttributeRef,
                    IsRequired = x.IsRequired,
                    IsVariant = x.IsVariant,
                    DisplayOrder = x.DisplayOrder,
                    IsOverridden = x.IsOverridden,
                    IsActive = x.IsActive
                })
                .ToListAsync()
            : new List<CategoryAttributeRuleResultItem>();

        return new GetCategoryByBusinessKeyQueryResult
        {
            CategoryBusinessKey = aggregate.BusinessKey,
            CurrentCategorySchemaVersionRef = currentSchemaVersionRef,
            ParentCategoryRef = aggregate.ParentCategoryRef,
            Code = aggregate.Code,
            Name = aggregate.Name,
            DisplayOrder = aggregate.DisplayOrder,
            IsActive = aggregate.IsActive,
            AttributeRules = rules
        };
    }

    public Task<GetCategoryByBusinessKeyQueryResult?> GetByIdAsync(Guid categoryId)
        => GetByBusinessKeyAsync(categoryId);

    public async Task<GetCategoryAttributesQueryResult?> GetAttributesAsync(Guid categoryBusinessKey, bool includeInherited = true, bool includeInactive = false)
    {
        var targetCategory = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == categoryBusinessKey);

        if (targetCategory is null)
            return null;

        var ruleItems = await GetCategoryAttributeRulesByCategoryIdAsync(categoryBusinessKey, includeInherited, includeInactive);

        return new GetCategoryAttributesQueryResult
        {
            CategoryBusinessKey = targetCategory.BusinessKey,
            CategoryCode = targetCategory.Code,
            CategoryName = targetCategory.Name,
            IncludeInherited = includeInherited,
            IncludeInactive = includeInactive,
            TotalCount = ruleItems.Count,
            Items = ruleItems.Select(x => new GetCategoryAttributeItem
            {
                AttributeRef = x.AttributeRef,
                AttributeCode = x.AttributeCode,
                AttributeName = x.AttributeName,
                DataType = x.DataType,
                Scope = x.Scope,
                AttributeIsActive = x.AttributeIsActive,
                RuleIsRequired = x.RuleIsRequired,
                RuleIsVariant = x.RuleIsVariant,
                RuleDisplayOrder = x.RuleDisplayOrder,
                RuleIsOverridden = x.RuleIsOverridden,
                RuleIsActive = x.RuleIsActive,
                IsInherited = x.IsInherited,
                SourceCategoryRef = x.SourceCategoryRef,
                SourceCategoryCode = x.SourceCategoryCode,
                SourceCategoryName = x.SourceCategoryName,
                Options = x.Options.Select(o => new GetCategoryAttributeOptionItem
                {
                    OptionBusinessKey = o.OptionBusinessKey,
                    Name = o.Name,
                    Value = o.Value,
                    DisplayOrder = o.DisplayOrder,
                    IsActive = o.IsActive
                }).ToList()
            }).ToList()
        };
    }

    public async Task<List<CategoryTreeItem>> GetTreeAsync(bool includeInactive = false)
    {
        var categories = await _dbContext.Set<CategoryReadModel>()
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        var items = categories.ToDictionary(
            x => x.BusinessKey,
            x => new CategoryTreeItem
            {
                CategoryBusinessKey = x.BusinessKey,
                ParentCategoryRef = x.ParentCategoryRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            });

        foreach (var item in items.Values)
        {
            if (item.ParentCategoryRef.HasValue && items.TryGetValue(item.ParentCategoryRef.Value, out var parent))
                parent.Children.Add(item);
        }

        return items.Values
            .Where(x => !x.ParentCategoryRef.HasValue || !items.ContainsKey(x.ParentCategoryRef.Value))
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public async Task<List<CategoryListItem>> GetRootCategoriesAsync(bool includeInactive = false)
    {
        return await _dbContext.Set<CategoryReadModel>()
            .Where(x => !x.ParentCategoryRef.HasValue)
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryListItem
            {
                CategoryBusinessKey = x.BusinessKey,
                ParentCategoryRef = x.ParentCategoryRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<CategoryListItem>> GetChildCategoriesAsync(Guid parentCategoryId, bool includeInactive = false)
    {
        return await _dbContext.Set<CategoryReadModel>()
            .Where(x => x.ParentCategoryRef == parentCategoryId)
            .Where(x => includeInactive || x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryListItem
            {
                CategoryBusinessKey = x.BusinessKey,
                ParentCategoryRef = x.ParentCategoryRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<CategoryBreadcrumbItem>> GetBreadcrumbAsync(Guid categoryId)
    {
        var items = new List<CategoryBreadcrumbItem>();
        var visited = new HashSet<Guid>();

        var current = await _dbContext.Set<CategoryReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == categoryId);
        while (current is not null && visited.Add(current.BusinessKey))
        {
            items.Add(new CategoryBreadcrumbItem
            {
                CategoryBusinessKey = current.BusinessKey,
                Code = current.Code,
                Name = current.Name,
                Depth = 0
            });

            current = current.ParentCategoryRef.HasValue
                ? await _dbContext.Set<CategoryReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == current.ParentCategoryRef.Value)
                : null;
        }

        items.Reverse();
        for (var i = 0; i < items.Count; i++)
            items[i].Depth = i;

        return items;
    }

    public async Task<SearchCategoriesQueryResult> SearchAsync(SearchCategoriesQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<CategoryReadModel> dbQuery = _dbContext.Set<CategoryReadModel>();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.Code, $"%{code}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));
        }

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(x => x.IsActive == query.IsActive.Value);

        if (query.ParentCategoryRef.HasValue)
            dbQuery = dbQuery.Where(x => x.ParentCategoryRef == query.ParentCategoryRef.Value);

        var totalCount = await dbQuery.CountAsync();

        var items = await dbQuery
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CategoryListItem
            {
                CategoryBusinessKey = x.BusinessKey,
                ParentCategoryRef = x.ParentCategoryRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchCategoriesQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<List<CategoryListItem>> GetActiveCategoriesAsync()
    {
        return await _dbContext.Set<CategoryReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryListItem
            {
                CategoryBusinessKey = x.BusinessKey,
                ParentCategoryRef = x.ParentCategoryRef,
                Code = x.Code,
                Name = x.Name,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<CategorySummaryItem?> GetSummaryAsync(Guid categoryId)
    {
        var category = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == categoryId);

        if (category is null)
            return null;

        var products = await _dbContext.Set<ProductReadModel>()
            .Where(x => x.CategoryRef == categoryId)
            .Select(x => new { x.BusinessKey, x.IsActive })
            .ToListAsync();

        var productIds = products.Select(x => x.BusinessKey).ToList();

        var variants = productIds.Count == 0
            ? new List<bool>()
            : await _dbContext.Set<ProductVariantReadModel>()
                .Where(x => productIds.Contains(x.ProductRef))
                .Select(x => x.IsActive)
                .ToListAsync();

        return new CategorySummaryItem
        {
            CategoryBusinessKey = category.BusinessKey,
            Code = category.Code,
            Name = category.Name,
            IsActive = category.IsActive,
            ProductCount = products.Count,
            ActiveProductCount = products.Count(x => x.IsActive),
            VariantCount = variants.Count,
            ActiveVariantCount = variants.Count(x => x)
        };
    }

    public async Task<CategoryAttributeRuleViewItem?> GetCategoryAttributeRuleByIdAsync(Guid categoryAttributeRuleId)
    {
        var rule = await _dbContext.Set<CategoryAttributeRuleReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == categoryAttributeRuleId);

        if (rule is null)
            return null;

        var schemaVersion = await _dbContext.Set<CategorySchemaVersionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == rule.CategorySchemaVersionRef);
        if (schemaVersion is null)
            return null;

        var category = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == schemaVersion.CategoryRef);
        if (category is null)
            return null;

        var definition = await _dbContext.Set<AttributeDefinitionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == rule.AttributeRef);
        if (definition is null)
            return null;

        var options = await _dbContext.Set<AttributeOptionReadModel>()
            .Where(x => x.AttributeRef == definition.BusinessKey)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryAttributeRuleOptionViewItem
            {
                OptionBusinessKey = x.BusinessKey,
                Name = string.IsNullOrWhiteSpace(x.Name) ? x.Value : x.Name,
                Value = x.Value,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new CategoryAttributeRuleViewItem
        {
            RuleBusinessKey = rule.BusinessKey,
            CategoryBusinessKey = category.BusinessKey,
            CategorySchemaVersionRef = rule.CategorySchemaVersionRef,
            AttributeRef = rule.AttributeRef,
            AttributeCode = definition.Code,
            AttributeName = definition.Name,
            DataType = definition.DataType.ToString(),
            Scope = definition.Scope.ToString(),
            AttributeIsActive = definition.IsActive,
            RuleIsRequired = rule.IsRequired,
            RuleIsVariant = rule.IsVariant,
            RuleDisplayOrder = rule.DisplayOrder,
            RuleIsOverridden = rule.IsOverridden,
            RuleIsActive = rule.IsActive,
            IsInherited = false,
            SourceCategoryRef = category.BusinessKey,
            SourceCategoryCode = category.Code,
            SourceCategoryName = category.Name,
            Options = options
        };
    }

    public async Task<List<CategoryAttributeRuleViewItem>> GetCategoryAttributeRulesByCategoryIdAsync(Guid categoryId, bool includeInherited = true, bool includeInactive = false)
    {
        var targetCategory = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == categoryId);

        if (targetCategory is null)
            return new List<CategoryAttributeRuleViewItem>();

        var lineage = await BuildLineageAsync(targetCategory, includeInherited);
        var lineageDepthByCategory = lineage.ToDictionary(x => x.Category.BusinessKey, x => x.Depth);
        var categoryRefs = lineageDepthByCategory.Keys.ToList();

        var schemaVersions = await _dbContext.Set<CategorySchemaVersionReadModel>()
            .Where(x => categoryRefs.Contains(x.CategoryRef))
            .ToListAsync();

        var schemaRefByCategory = schemaVersions
            .Where(x => x.IsCurrent)
            .GroupBy(x => x.CategoryRef)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(v => v.VersionNo).First().BusinessKey);

        var schemaRefs = schemaRefByCategory.Values.Distinct().ToList();
        if (schemaRefs.Count == 0)
            return new List<CategoryAttributeRuleViewItem>();

        var sourceCategoryBySchemaRef = schemaVersions
            .Where(x => schemaRefs.Contains(x.BusinessKey))
            .GroupBy(x => x.BusinessKey)
            .ToDictionary(x => x.Key, x => x.First().CategoryRef);

        var allRules = await _dbContext.Set<CategoryAttributeRuleReadModel>()
            .Where(x => schemaRefs.Contains(x.CategorySchemaVersionRef))
            .ToListAsync();

        allRules = allRules
            .Where(x => sourceCategoryBySchemaRef.ContainsKey(x.CategorySchemaVersionRef))
            .ToList();

        var resolvedRules = allRules
            .GroupBy(x => x.AttributeRef)
            .Select(group =>
            {
                var selected = group
                    .Select(rule => new
                    {
                        Rule = rule,
                        SourceCategoryRef = sourceCategoryBySchemaRef[rule.CategorySchemaVersionRef]
                    })
                    .OrderBy(rule => lineageDepthByCategory[rule.SourceCategoryRef])
                    .ThenByDescending(rule => rule.Rule.IsOverridden)
                    .ThenBy(rule => rule.Rule.DisplayOrder)
                    .First();

                return selected.Rule;
            })
            .ToList();

        if (!includeInactive)
            resolvedRules = resolvedRules.Where(x => x.IsActive).ToList();

        var attributeRefs = resolvedRules.Select(x => x.AttributeRef).Distinct().ToList();
        if (attributeRefs.Count == 0)
            return new List<CategoryAttributeRuleViewItem>();

        var definitions = await _dbContext.Set<AttributeDefinitionReadModel>()
            .Where(x => attributeRefs.Contains(x.BusinessKey))
            .ToListAsync();

        if (!includeInactive)
            definitions = definitions.Where(x => x.IsActive).ToList();

        var definitionMap = definitions.ToDictionary(x => x.BusinessKey, x => x);
        resolvedRules = resolvedRules.Where(x => definitionMap.ContainsKey(x.AttributeRef)).ToList();

        var optionsQuery = _dbContext.Set<AttributeOptionReadModel>()
            .Where(x => attributeRefs.Contains(x.AttributeRef));

        if (!includeInactive)
            optionsQuery = optionsQuery.Where(x => x.IsActive);

        var options = await optionsQuery
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();

        var optionsByAttribute = options
            .GroupBy(x => x.AttributeRef)
            .ToDictionary(
                x => x.Key,
                x => x.Select(o => new CategoryAttributeRuleOptionViewItem
                {
                    OptionBusinessKey = o.BusinessKey,
                    Name = string.IsNullOrWhiteSpace(o.Name) ? o.Value : o.Name,
                    Value = o.Value,
                    DisplayOrder = o.DisplayOrder,
                    IsActive = o.IsActive
                }).ToList());

        var categoryMap = lineage.ToDictionary(x => x.Category.BusinessKey, x => x.Category);

        return resolvedRules
            .Select(rule =>
            {
                var definition = definitionMap[rule.AttributeRef];
                var sourceCategoryRef = sourceCategoryBySchemaRef[rule.CategorySchemaVersionRef];
                var sourceCategory = categoryMap[sourceCategoryRef];

                return new CategoryAttributeRuleViewItem
                {
                    RuleBusinessKey = rule.BusinessKey,
                    CategoryBusinessKey = categoryId,
                    CategorySchemaVersionRef = rule.CategorySchemaVersionRef,
                    AttributeRef = rule.AttributeRef,
                    AttributeCode = definition.Code,
                    AttributeName = definition.Name,
                    DataType = definition.DataType.ToString(),
                    Scope = definition.Scope.ToString(),
                    AttributeIsActive = definition.IsActive,
                    RuleIsRequired = rule.IsRequired,
                    RuleIsVariant = rule.IsVariant,
                    RuleDisplayOrder = rule.DisplayOrder,
                    RuleIsOverridden = rule.IsOverridden,
                    RuleIsActive = rule.IsActive,
                    IsInherited = sourceCategoryRef != categoryId,
                    SourceCategoryRef = sourceCategory.BusinessKey,
                    SourceCategoryCode = sourceCategory.Code,
                    SourceCategoryName = sourceCategory.Name,
                    Options = optionsByAttribute.TryGetValue(rule.AttributeRef, out var value)
                        ? value
                        : new List<CategoryAttributeRuleOptionViewItem>()
                };
            })
            .OrderBy(x => x.RuleIsVariant)
            .ThenBy(x => x.RuleDisplayOrder)
            .ThenBy(x => x.AttributeName)
            .ToList();
    }

    private async Task<List<CategoryLineageItem>> BuildLineageAsync(CategoryReadModel category, bool includeInherited)
    {
        var lineage = new List<CategoryLineageItem>();
        var visited = new HashSet<Guid>();

        var current = category;
        var depth = 0;

        while (current is not null && visited.Add(current.BusinessKey))
        {
            lineage.Add(new CategoryLineageItem(current, depth));

            if (!includeInherited || !current.ParentCategoryRef.HasValue)
                break;

            current = await _dbContext.Set<CategoryReadModel>()
                .FirstOrDefaultAsync(x => x.BusinessKey == current.ParentCategoryRef.Value);

            depth++;
        }

        return lineage;
    }

    private sealed record CategoryLineageItem(CategoryReadModel Category, int Depth);
}
