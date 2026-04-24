namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class ProductQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IProductQueryRepository
{
    public ProductQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<GetProductByBusinessKeyQueryResult?> GetByIdAsync(Guid productId)
        => GetByBusinessKeyAsync(productId);

    public async Task<GetProductByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid productBusinessKey)
    {
        var aggregate = await _dbContext.Set<ProductReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == productBusinessKey);

        if (aggregate is null)
            return null;

        var attributes = await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productBusinessKey)
            .OrderBy(x => x.AttributeRef)
            .Select(x => new ProductAttributeValueResultItem
            {
                AttributeRef = x.AttributeRef,
                Value = x.Value,
                OptionRef = x.OptionRef
            })
            .ToListAsync();

        return new GetProductByBusinessKeyQueryResult
        {
            ProductBusinessKey = aggregate.BusinessKey,
            CategoryRef = aggregate.CategoryRef,
            CategorySchemaVersionRef = aggregate.CategorySchemaVersionRef,
            BaseSku = aggregate.BaseSku,
            Name = aggregate.Name,
            DefaultUomRef = aggregate.DefaultUomRef,
            TaxCategoryRef = aggregate.TaxCategoryRef,
            IsActive = aggregate.IsActive,
            AttributeValues = attributes
        };
    }

    public async Task<SearchProductsQueryResult> SearchAsync(SearchProductsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<ProductReadModel> dbQuery = _dbContext.Set<ProductReadModel>();

        if (query.CategoryRef.HasValue)
            dbQuery = dbQuery.Where(x => x.CategoryRef == query.CategoryRef.Value);

        if (!string.IsNullOrWhiteSpace(query.BaseSku))
        {
            var baseSku = query.BaseSku.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.BaseSku, $"%{baseSku}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));
        }

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .OrderBy(x => x.Name)
            .ThenBy(x => x.BaseSku)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToProductListItem(x))
            .ToListAsync();

        return new SearchProductsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<List<ProductListItem>> GetByCategoryIdAsync(Guid categoryId, bool includeInactive = false)
    {
        var query = _dbContext.Set<ProductReadModel>().Where(x => x.CategoryRef == categoryId);

        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.BaseSku)
            .Select(x => ToProductListItem(x))
            .ToListAsync();
    }

    public async Task<List<ProductListItem>> GetActiveAsync()
    {
        return await _dbContext.Set<ProductReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.BaseSku)
            .Select(x => ToProductListItem(x))
            .ToListAsync();
    }

    public async Task<ProductSummaryItem?> GetSummaryAsync(Guid productId)
    {
        var product = await _dbContext.Set<ProductReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == productId);

        if (product is null)
            return null;

        var variants = await _dbContext.Set<ProductVariantReadModel>()
            .Where(x => x.ProductRef == productId)
            .Select(x => x.IsActive)
            .ToListAsync();

        var attrCount = await _dbContext.Set<ProductAttributeValueReadModel>()
            .CountAsync(x => x.ProductRef == productId);

        return new ProductSummaryItem
        {
            ProductBusinessKey = product.BusinessKey,
            BaseSku = product.BaseSku,
            Name = product.Name,
            IsActive = product.IsActive,
            VariantCount = variants.Count,
            ActiveVariantCount = variants.Count(x => x),
            AttributeValueCount = attrCount
        };
    }

    public async Task<ProductDetailsWithAttributesItem?> GetDetailsWithAttributesAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return null;

        var values = await GetAttributeValuesByProductIdAsync(productId);

        return new ProductDetailsWithAttributesItem
        {
            Product = ToProductListItem(product),
            AttributeValues = values
        };
    }

    public async Task<ProductDetailsWithVariantsItem?> GetDetailsWithVariantsAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return null;

        var variants = await GetVariantsByProductIdAsync(productId, includeInactive: true);

        return new ProductDetailsWithVariantsItem
        {
            Product = ToProductListItem(product),
            Variants = variants
        };
    }

    public async Task<ProductFullDetailsItem?> GetFullDetailsAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return null;

        var category = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == product.CategoryRef);

        var productAttributes = await GetAttributeValuesWithDefinitionByProductIdAsync(productId);
        var variants = await GetVariantsWithAttributesAsync(productId);

        return new ProductFullDetailsItem
        {
            Product = ToProductListItem(product),
            CategoryBusinessKey = category?.BusinessKey,
            CategoryCode = category?.Code,
            CategoryName = category?.Name,
            ProductAttributes = productAttributes,
            Variants = variants
        };
    }

    public async Task<List<ProductAttributeValueViewItem>> GetAttributeValuesByProductIdAsync(Guid productId)
    {
        return await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productId)
            .OrderBy(x => x.AttributeRef)
            .Select(x => ToProductAttributeValueItem(x))
            .ToListAsync();
    }

    public async Task<ProductAttributeValueViewItem?> GetAttributeValueByIdAsync(Guid productAttributeValueId)
    {
        var item = await _dbContext.Set<ProductAttributeValueReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == productAttributeValueId);

        return item is null ? null : ToProductAttributeValueItem(item);
    }

    public async Task<List<ProductAttributeValueWithDefinitionItem>> GetAttributeValuesWithDefinitionByProductIdAsync(Guid productId)
    {
        var values = await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productId)
            .OrderBy(x => x.AttributeRef)
            .ToListAsync();

        var attributeRefs = values.Select(x => x.AttributeRef).Distinct().ToList();

        var definitions = attributeRefs.Count == 0
            ? new List<AttributeDefinitionReadModel>()
            : await _dbContext.Set<AttributeDefinitionReadModel>()
                .Where(x => attributeRefs.Contains(x.BusinessKey))
                .ToListAsync();

        var options = await _dbContext.Set<AttributeOptionReadModel>()
            .Where(x => x.AttributeRef != Guid.Empty)
            .ToListAsync();

        var defMap = definitions.ToDictionary(x => x.BusinessKey, x => x);
        var optionMap = options.ToDictionary(x => x.BusinessKey, x => x.Value);

        return values.Select(x =>
        {
            defMap.TryGetValue(x.AttributeRef, out var def);
            optionMap.TryGetValue(x.OptionRef ?? Guid.Empty, out var optionValue);

            return new ProductAttributeValueWithDefinitionItem
            {
                ProductAttributeValueBusinessKey = x.BusinessKey,
                ProductRef = x.ProductRef,
                AttributeRef = x.AttributeRef,
                AttributeCode = def?.Code ?? string.Empty,
                AttributeName = def?.Name ?? string.Empty,
                DataType = def?.DataType.ToString() ?? string.Empty,
                Scope = def?.Scope.ToString() ?? string.Empty,
                Value = x.Value,
                OptionRef = x.OptionRef,
                OptionValue = x.OptionRef.HasValue ? optionValue : null
            };
        }).ToList();
    }

    public async Task<List<MissingRequiredProductAttributeItem>> GetMissingRequiredAttributesAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return new List<MissingRequiredProductAttributeItem>();

        var requiredRules = await GetEffectiveCategoryRulesAsync(product.CategoryRef, product.CategorySchemaVersionRef, isVariant: false, requiredOnly: true);
        var existingAttributeRefs = await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productId)
            .Select(x => x.AttributeRef)
            .Distinct()
            .ToListAsync();

        return requiredRules
            .Where(x => !existingAttributeRefs.Contains(x.AttributeBusinessKey))
            .Select(x => new MissingRequiredProductAttributeItem
            {
                AttributeRef = x.AttributeBusinessKey,
                AttributeCode = x.AttributeCode,
                AttributeName = x.AttributeName,
                DataType = x.DataType,
                Scope = x.Scope
            })
            .ToList();
    }

    public async Task<ProductCatalogFormItem?> GetCatalogFormAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return null;

        var rules = await GetEffectiveCategoryRulesAsync(product.CategoryRef, product.CategorySchemaVersionRef, isVariant: false, requiredOnly: false);
        var valueMap = await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productId)
            .ToDictionaryAsync(x => x.AttributeRef, x => x);

        return new ProductCatalogFormItem
        {
            ProductBusinessKey = product.BusinessKey,
            CategoryBusinessKey = product.CategoryRef,
            CategorySchemaVersionRef = product.CategorySchemaVersionRef,
            Fields = rules
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.AttributeName)
                .Select(x =>
                {
                    valueMap.TryGetValue(x.AttributeBusinessKey, out var value);
                    return new ProductFormFieldItem
                    {
                        AttributeRef = x.AttributeBusinessKey,
                        AttributeCode = x.AttributeCode,
                        AttributeName = x.AttributeName,
                        DataType = x.DataType,
                        IsRequired = x.IsRequired,
                        DisplayOrder = x.DisplayOrder,
                        Value = value?.Value,
                        OptionRef = value?.OptionRef
                    };
                })
                .ToList()
        };
    }

    public async Task<ProductCompletionStatusItem?> GetCompletionStatusAsync(Guid productId)
    {
        var product = await GetProductOrNull(productId);
        if (product is null)
            return null;

        var requiredRules = await GetEffectiveCategoryRulesAsync(product.CategoryRef, product.CategorySchemaVersionRef, isVariant: false, requiredOnly: true);
        var existingAttributeRefs = await _dbContext.Set<ProductAttributeValueReadModel>()
            .Where(x => x.ProductRef == productId)
            .Select(x => x.AttributeRef)
            .Distinct()
            .ToListAsync();

        var missing = requiredRules
            .Where(x => !existingAttributeRefs.Contains(x.AttributeBusinessKey))
            .Select(x => new MissingRequiredProductAttributeItem
            {
                AttributeRef = x.AttributeBusinessKey,
                AttributeCode = x.AttributeCode,
                AttributeName = x.AttributeName,
                DataType = x.DataType,
                Scope = x.Scope
            })
            .ToList();

        var requiredCount = requiredRules.Count;
        var missingCount = missing.Count;
        var completed = requiredCount - missingCount;

        return new ProductCompletionStatusItem
        {
            ProductBusinessKey = product.BusinessKey,
            RequiredCount = requiredCount,
            CompletedCount = completed,
            MissingCount = missingCount,
            IsComplete = missingCount == 0,
            MissingAttributes = missing
        };
    }

    public async Task<ProductEditorDataItem?> GetEditorDataAsync(Guid productId)
    {
        var details = await GetFullDetailsAsync(productId);
        if (details is null)
            return null;

        var form = await GetCatalogFormAsync(productId);
        var completion = await GetCompletionStatusAsync(productId);

        if (form is null || completion is null)
            return null;

        return new ProductEditorDataItem
        {
            ProductDetails = details,
            CatalogForm = form,
            CompletionStatus = completion
        };
    }

    private async Task<ProductReadModel?> GetProductOrNull(Guid productId)
        => await _dbContext.Set<ProductReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == productId);

    private async Task<List<ProductVariantListItem>> GetVariantsByProductIdAsync(Guid productId, bool includeInactive)
    {
        var query = _dbContext.Set<ProductVariantReadModel>().Where(x => x.ProductRef == productId);
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.VariantSku)
            .Select(x => ToVariantListItem(x))
            .ToListAsync();
    }

    private async Task<List<ProductVariantFullItem>> GetVariantsWithAttributesAsync(Guid productId)
    {
        var variants = await _dbContext.Set<ProductVariantReadModel>()
            .Where(x => x.ProductRef == productId)
            .OrderBy(x => x.VariantSku)
            .ToListAsync();

        if (variants.Count == 0)
            return new List<ProductVariantFullItem>();

        var variantIds = variants.Select(x => x.BusinessKey).ToList();
        var values = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => variantIds.Contains(x.VariantRef))
            .ToListAsync();

        var attributeRefs = values.Select(x => x.AttributeRef).Distinct().ToList();
        var defs = attributeRefs.Count == 0
            ? new List<AttributeDefinitionReadModel>()
            : await _dbContext.Set<AttributeDefinitionReadModel>()
                .Where(x => attributeRefs.Contains(x.BusinessKey))
                .ToListAsync();

        var optionIds = values.Where(x => x.OptionRef.HasValue).Select(x => x.OptionRef!.Value).Distinct().ToList();
        var options = optionIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _dbContext.Set<AttributeOptionReadModel>()
                .Where(x => optionIds.Contains(x.BusinessKey))
                .ToDictionaryAsync(x => x.BusinessKey, x => x.Value);

        var defMap = defs.ToDictionary(x => x.BusinessKey, x => x);
        var valuesByVariant = values.GroupBy(x => x.VariantRef).ToDictionary(x => x.Key, x => x.ToList());

        return variants.Select(v =>
        {
            var attrs = valuesByVariant.TryGetValue(v.BusinessKey, out var list) ? list : new List<VariantAttributeValueReadModel>();
            return new ProductVariantFullItem
            {
                Variant = ToVariantListItem(v),
                Attributes = attrs.Select(a =>
                {
                    defMap.TryGetValue(a.AttributeRef, out var def);
                    options.TryGetValue(a.OptionRef ?? Guid.Empty, out var optionValue);
                    return new VariantAttributeValueInlineItem
                    {
                        VariantAttributeValueBusinessKey = a.BusinessKey,
                        VariantRef = a.VariantRef,
                        AttributeRef = a.AttributeRef,
                        AttributeCode = def?.Code ?? string.Empty,
                        AttributeName = def?.Name ?? string.Empty,
                        DataType = def?.DataType.ToString() ?? string.Empty,
                        Scope = def?.Scope.ToString() ?? string.Empty,
                        Value = a.Value,
                        OptionRef = a.OptionRef,
                        OptionValue = a.OptionRef.HasValue ? optionValue : null
                    };
                }).ToList()
            };
        }).ToList();
    }

    private async Task<List<ResolvedCategoryRule>> GetEffectiveCategoryRulesAsync(
        Guid categoryId,
        Guid categorySchemaVersionRef,
        bool isVariant,
        bool requiredOnly)
    {
        var lineage = await BuildLineageAsync(categoryId);
        if (lineage.Count == 0)
            return new List<ResolvedCategoryRule>();

        var lineageDepth = lineage.ToDictionary(x => x.BusinessKey, x => x.Depth);
        var categoryRefs = lineage.Select(x => x.BusinessKey).Distinct().ToList();

        var schemaVersions = await _dbContext.Set<CategorySchemaVersionReadModel>()
            .Where(x => categoryRefs.Contains(x.CategoryRef))
            .ToListAsync();

        var schemaRefByCategory = schemaVersions
            .Where(x => x.IsCurrent)
            .GroupBy(x => x.CategoryRef)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(v => v.VersionNo).First().BusinessKey);

        if (categorySchemaVersionRef != Guid.Empty)
        {
            var requestedVersion = schemaVersions.FirstOrDefault(x => x.BusinessKey == categorySchemaVersionRef && x.CategoryRef == categoryId);
            if (requestedVersion is not null)
                schemaRefByCategory[categoryId] = requestedVersion.BusinessKey;
        }

        var schemaRefs = schemaRefByCategory.Values.Distinct().ToList();
        if (schemaRefs.Count == 0)
            return new List<ResolvedCategoryRule>();

        var categoryRefBySchema = schemaVersions
            .Where(x => schemaRefs.Contains(x.BusinessKey))
            .GroupBy(x => x.BusinessKey)
            .ToDictionary(x => x.Key, x => x.First().CategoryRef);

        var rules = await _dbContext.Set<CategoryAttributeRuleReadModel>()
            .Where(x => schemaRefs.Contains(x.CategorySchemaVersionRef) && x.IsActive)
            .ToListAsync();

        rules = rules
            .Where(x => categoryRefBySchema.ContainsKey(x.CategorySchemaVersionRef))
            .ToList();

        rules = rules.Where(x => x.IsVariant == isVariant).ToList();
        if (requiredOnly)
            rules = rules.Where(x => x.IsRequired).ToList();

        var resolved = rules
            .GroupBy(x => x.AttributeRef)
            .Select(g =>
            {
                var selected = g
                    .Select(r => new
                    {
                        Rule = r,
                        SourceCategoryRef = categoryRefBySchema[r.CategorySchemaVersionRef]
                    })
                    .OrderBy(r => lineageDepth[r.SourceCategoryRef])
                    .ThenByDescending(r => r.Rule.IsOverridden)
                    .ThenBy(r => r.Rule.DisplayOrder)
                    .First();

                return selected.Rule;
            })
            .ToList();

        var attrIds = resolved.Select(x => x.AttributeRef).Distinct().ToList();
        var defs = attrIds.Count == 0
            ? new List<AttributeDefinitionReadModel>()
            : await _dbContext.Set<AttributeDefinitionReadModel>()
                .Where(x => attrIds.Contains(x.BusinessKey) && x.IsActive)
                .ToListAsync();

        var defMap = defs.ToDictionary(x => x.BusinessKey, x => x);

        return resolved
            .Where(r => defMap.ContainsKey(r.AttributeRef))
            .Select(r =>
            {
                var def = defMap[r.AttributeRef];
                return new ResolvedCategoryRule
                {
                    AttributeBusinessKey = r.AttributeRef,
                    AttributeCode = def.Code,
                    AttributeName = def.Name,
                    DataType = def.DataType.ToString(),
                    Scope = def.Scope.ToString(),
                    IsRequired = r.IsRequired,
                    DisplayOrder = r.DisplayOrder
                };
            })
            .ToList();
    }

    private async Task<List<CategoryLineageNode>> BuildLineageAsync(Guid categoryId)
    {
        var result = new List<CategoryLineageNode>();
        var visited = new HashSet<Guid>();

        var current = await _dbContext.Set<CategoryReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == categoryId);
        var depth = 0;

        while (current is not null && visited.Add(current.BusinessKey))
        {
            result.Add(new CategoryLineageNode(current.BusinessKey, depth));

            current = current.ParentCategoryRef.HasValue
                ? await _dbContext.Set<CategoryReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == current.ParentCategoryRef.Value)
                : null;

            depth++;
        }

        return result;
    }

    private static ProductListItem ToProductListItem(ProductReadModel x)
        => new()
        {
            ProductBusinessKey = x.BusinessKey,
            CategoryRef = x.CategoryRef,
            CategorySchemaVersionRef = x.CategorySchemaVersionRef,
            BaseSku = x.BaseSku,
            Name = x.Name,
            DefaultUomRef = x.DefaultUomRef,
            TaxCategoryRef = x.TaxCategoryRef,
            IsActive = x.IsActive
        };

    private static ProductVariantListItem ToVariantListItem(ProductVariantReadModel x)
        => new()
        {
            VariantBusinessKey = x.BusinessKey,
            VariantSku = x.VariantSku,
            Barcode = x.Barcode,
            TrackingPolicy = x.TrackingPolicy.ToString(),
            BaseUomRef = x.BaseUomRef,
            IsActive = x.IsActive
        };

    private static ProductAttributeValueViewItem ToProductAttributeValueItem(ProductAttributeValueReadModel x)
        => new()
        {
            ProductAttributeValueBusinessKey = x.BusinessKey,
            ProductRef = x.ProductRef,
            AttributeRef = x.AttributeRef,
            Value = x.Value,
            OptionRef = x.OptionRef
        };

    private sealed class ResolvedCategoryRule
    {
        public Guid AttributeBusinessKey { get; set; }
        public string AttributeCode { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }

    private sealed record CategoryLineageNode(Guid BusinessKey, int Depth);
}
