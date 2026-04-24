namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class ProductVariantQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IProductVariantQueryRepository
{
    public ProductVariantQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<GetProductVariantByBusinessKeyQueryResult?> GetByIdAsync(Guid variantId)
        => GetByBusinessKeyAsync(variantId);

    public async Task<GetProductVariantByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid productVariantBusinessKey)
    {
        var aggregate = await _dbContext.Set<ProductVariantReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == productVariantBusinessKey);

        if (aggregate is null)
            return null;

        var attributes = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == productVariantBusinessKey)
            .OrderBy(x => x.AttributeRef)
            .Select(x => new VariantAttributeValueResultItem
            {
                AttributeRef = x.AttributeRef,
                Value = x.Value,
                OptionRef = x.OptionRef
            })
            .ToListAsync();

        var conversions = await _dbContext.Set<VariantUomConversionReadModel>()
            .Where(x => x.VariantRef == productVariantBusinessKey)
            .OrderBy(x => x.FromUomRef)
            .ThenBy(x => x.ToUomRef)
            .Select(x => new VariantUomConversionResultItem
            {
                FromUomRef = x.FromUomRef,
                ToUomRef = x.ToUomRef,
                Factor = x.Factor,
                RoundingMode = x.RoundingMode.ToString(),
                IsBasePath = x.IsBasePath
            })
            .ToListAsync();

        return new GetProductVariantByBusinessKeyQueryResult
        {
            ProductVariantBusinessKey = aggregate.BusinessKey,
            ProductRef = aggregate.ProductRef,
            VariantSku = aggregate.VariantSku,
            Barcode = aggregate.Barcode,
            TrackingPolicy = aggregate.TrackingPolicy.ToString(),
            BaseUomRef = aggregate.BaseUomRef,
            IsActive = aggregate.IsActive,
            InventoryMovementLocked = aggregate.InventoryMovementLocked,
            AttributeValues = attributes,
            UomConversions = conversions
        };
    }

    public async Task<VariantListItem?> GetBySkuAsync(string variantSku)
    {
        if (string.IsNullOrWhiteSpace(variantSku))
            return null;

        var sku = variantSku.Trim();
        var item = await _dbContext.Set<ProductVariantReadModel>()
            .FirstOrDefaultAsync(x => x.VariantSku == sku);

        return item is null ? null : ToVariantListItem(item);
    }

    public async Task<VariantListItem?> GetByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return null;

        var code = barcode.Trim();
        var item = await _dbContext.Set<ProductVariantReadModel>()
            .FirstOrDefaultAsync(x => x.Barcode == code);

        return item is null ? null : ToVariantListItem(item);
    }

    public async Task<List<VariantListItem>> GetByProductIdAsync(Guid productId, bool includeInactive = false)
    {
        var query = _dbContext.Set<ProductVariantReadModel>().Where(x => x.ProductRef == productId);
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.VariantSku)
            .Select(x => ToVariantListItem(x))
            .ToListAsync();
    }

    public async Task<SearchVariantsQueryResult> SearchAsync(SearchVariantsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<ProductVariantReadModel> dbQuery = _dbContext.Set<ProductVariantReadModel>();

        if (query.ProductRef.HasValue)
            dbQuery = dbQuery.Where(x => x.ProductRef == query.ProductRef.Value);

        if (!string.IsNullOrWhiteSpace(query.VariantSku))
        {
            var sku = query.VariantSku.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.VariantSku, $"%{sku}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Barcode))
        {
            var barcode = query.Barcode.Trim();
            dbQuery = dbQuery.Where(x => x.Barcode != null && EF.Functions.ILike(x.Barcode, $"%{barcode}%"));
        }

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .OrderBy(x => x.VariantSku)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToVariantListItem(x))
            .ToListAsync();

        return new SearchVariantsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<List<VariantListItem>> GetActiveAsync()
    {
        return await _dbContext.Set<ProductVariantReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.VariantSku)
            .Select(x => ToVariantListItem(x))
            .ToListAsync();
    }

    public async Task<VariantSummaryItem?> GetSummaryAsync(Guid variantId)
    {
        var variant = await GetVariantOrNull(variantId);
        if (variant is null)
            return null;

        var attrCount = await _dbContext.Set<VariantAttributeValueReadModel>()
            .CountAsync(x => x.VariantRef == variantId);

        return new VariantSummaryItem
        {
            VariantBusinessKey = variant.BusinessKey,
            VariantSku = variant.VariantSku,
            Barcode = variant.Barcode,
            IsActive = variant.IsActive,
            AttributeValueCount = attrCount
        };
    }

    public async Task<VariantDetailsWithAttributesItem?> GetDetailsWithAttributesAsync(Guid variantId)
    {
        var variant = await GetVariantOrNull(variantId);
        if (variant is null)
            return null;

        var values = await GetAttributeValuesByVariantIdAsync(variantId);

        return new VariantDetailsWithAttributesItem
        {
            Variant = ToVariantListItem(variant),
            AttributeValues = values
        };
    }

    public async Task<VariantDetailsWithProductContextItem?> GetDetailsWithProductContextAsync(Guid variantId)
    {
        var variant = await GetVariantOrNull(variantId);
        if (variant is null)
            return null;

        var product = await _dbContext.Set<ProductReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == variant.ProductRef);
        if (product is null)
            return null;

        var category = await _dbContext.Set<CategoryReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == product.CategoryRef);
        if (category is null)
            return null;

        return new VariantDetailsWithProductContextItem
        {
            Variant = ToVariantListItem(variant),
            ProductBusinessKey = product.BusinessKey,
            ProductName = product.Name,
            ProductBaseSku = product.BaseSku,
            CategoryBusinessKey = category.BusinessKey,
            CategoryCode = category.Code,
            CategoryName = category.Name
        };
    }

    public async Task<VariantFullDetailsItem?> GetFullDetailsAsync(Guid variantId)
    {
        var context = await GetDetailsWithProductContextAsync(variantId);
        if (context is null)
            return null;

        var attrs = await GetAttributeValuesWithDefinitionByVariantIdAsync(variantId);

        return new VariantFullDetailsItem
        {
            Variant = context.Variant,
            ProductBusinessKey = context.ProductBusinessKey,
            ProductName = context.ProductName,
            ProductBaseSku = context.ProductBaseSku,
            CategoryBusinessKey = context.CategoryBusinessKey,
            CategoryCode = context.CategoryCode,
            CategoryName = context.CategoryName,
            AttributeValues = attrs
        };
    }

    public async Task<List<VariantAttributeValueViewItem>> GetAttributeValuesByVariantIdAsync(Guid variantId)
    {
        return await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == variantId)
            .OrderBy(x => x.AttributeRef)
            .Select(x => ToVariantAttributeValueItem(x))
            .ToListAsync();
    }

    public async Task<VariantAttributeValueViewItem?> GetAttributeValueByIdAsync(Guid variantAttributeValueId)
    {
        var value = await _dbContext.Set<VariantAttributeValueReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == variantAttributeValueId);

        return value is null ? null : ToVariantAttributeValueItem(value);
    }

    public async Task<List<VariantAttributeValueWithDefinitionItem>> GetAttributeValuesWithDefinitionByVariantIdAsync(Guid variantId)
    {
        var values = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == variantId)
            .OrderBy(x => x.AttributeRef)
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

        return values.Select(x =>
        {
            defMap.TryGetValue(x.AttributeRef, out var def);
            options.TryGetValue(x.OptionRef ?? Guid.Empty, out var optionValue);

            return new VariantAttributeValueWithDefinitionItem
            {
                VariantAttributeValueBusinessKey = x.BusinessKey,
                VariantRef = x.VariantRef,
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

    public async Task<List<VariantUomConversionViewItem>> GetUomConversionsByVariantIdAsync(Guid variantId)
    {
        return await _dbContext.Set<VariantUomConversionReadModel>()
            .Where(x => x.VariantRef == variantId)
            .OrderBy(x => x.FromUomRef)
            .ThenBy(x => x.ToUomRef)
            .Select(x => ToVariantUomConversionItem(x))
            .ToListAsync();
    }

    public async Task<VariantUomConversionViewItem?> GetUomConversionByPathAsync(Guid variantId, Guid fromUomRef, Guid toUomRef)
    {
        var conversion = await _dbContext.Set<VariantUomConversionReadModel>()
            .FirstOrDefaultAsync(x => x.VariantRef == variantId && x.FromUomRef == fromUomRef && x.ToUomRef == toUomRef);

        return conversion is null ? null : ToVariantUomConversionItem(conversion);
    }

    public async Task<List<MissingRequiredVariantAttributeItem>> GetMissingRequiredAttributesAsync(Guid variantId)
    {
        var ctx = await GetVariantProductCategoryContextAsync(variantId);
        if (ctx is null)
            return new List<MissingRequiredVariantAttributeItem>();

        var requiredRules = await GetEffectiveCategoryRulesAsync(ctx.CategoryRef, ctx.CategorySchemaVersionRef, isVariant: true, requiredOnly: true);

        var existing = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == variantId)
            .Select(x => x.AttributeRef)
            .Distinct()
            .ToListAsync();

        return requiredRules
            .Where(x => !existing.Contains(x.AttributeBusinessKey))
            .Select(x => new MissingRequiredVariantAttributeItem
            {
                AttributeRef = x.AttributeBusinessKey,
                AttributeCode = x.AttributeCode,
                AttributeName = x.AttributeName,
                DataType = x.DataType,
                Scope = x.Scope
            })
            .ToList();
    }

    public async Task<VariantCatalogFormItem?> GetCatalogFormAsync(Guid variantId)
    {
        var ctx = await GetVariantProductCategoryContextAsync(variantId);
        if (ctx is null)
            return null;

        var rules = await GetEffectiveCategoryRulesAsync(ctx.CategoryRef, ctx.CategorySchemaVersionRef, isVariant: true, requiredOnly: false);
        var valueMap = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == variantId)
            .ToDictionaryAsync(x => x.AttributeRef, x => x);

        return new VariantCatalogFormItem
        {
            VariantBusinessKey = variantId,
            ProductBusinessKey = ctx.ProductRef,
            CategoryBusinessKey = ctx.CategoryRef,
            Fields = rules
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.AttributeName)
                .Select(x =>
                {
                    valueMap.TryGetValue(x.AttributeBusinessKey, out var value);
                    return new VariantFormFieldItem
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
                }).ToList()
        };
    }

    public async Task<VariantCompletionStatusItem?> GetCompletionStatusAsync(Guid variantId)
    {
        var ctx = await GetVariantProductCategoryContextAsync(variantId);
        if (ctx is null)
            return null;

        var requiredRules = await GetEffectiveCategoryRulesAsync(ctx.CategoryRef, ctx.CategorySchemaVersionRef, isVariant: true, requiredOnly: true);
        var existing = await _dbContext.Set<VariantAttributeValueReadModel>()
            .Where(x => x.VariantRef == variantId)
            .Select(x => x.AttributeRef)
            .Distinct()
            .ToListAsync();

        var missing = requiredRules
            .Where(x => !existing.Contains(x.AttributeBusinessKey))
            .Select(x => new MissingRequiredVariantAttributeItem
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

        return new VariantCompletionStatusItem
        {
            VariantBusinessKey = variantId,
            RequiredCount = requiredCount,
            CompletedCount = requiredCount - missingCount,
            MissingCount = missingCount,
            IsComplete = missingCount == 0,
            MissingAttributes = missing
        };
    }

    public async Task<VariantEditorDataItem?> GetEditorDataAsync(Guid variantId)
    {
        var details = await GetFullDetailsAsync(variantId);
        if (details is null)
            return null;

        var form = await GetCatalogFormAsync(variantId);
        var completion = await GetCompletionStatusAsync(variantId);

        if (form is null || completion is null)
            return null;

        return new VariantEditorDataItem
        {
            VariantDetails = details,
            CatalogForm = form,
            CompletionStatus = completion
        };
    }

    private async Task<ProductVariantReadModel?> GetVariantOrNull(Guid variantId)
        => await _dbContext.Set<ProductVariantReadModel>().FirstOrDefaultAsync(x => x.BusinessKey == variantId);

    private async Task<VariantProductCategoryContext?> GetVariantProductCategoryContextAsync(Guid variantId)
    {
        var variant = await _dbContext.Set<ProductVariantReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == variantId);
        if (variant is null)
            return null;

        var product = await _dbContext.Set<ProductReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == variant.ProductRef);
        if (product is null)
            return null;

        return new VariantProductCategoryContext
        {
            VariantRef = variant.BusinessKey,
            ProductRef = product.BusinessKey,
            CategoryRef = product.CategoryRef,
            CategorySchemaVersionRef = product.CategorySchemaVersionRef
        };
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

    private static VariantListItem ToVariantListItem(ProductVariantReadModel x)
        => new()
        {
            VariantBusinessKey = x.BusinessKey,
            ProductRef = x.ProductRef,
            VariantSku = x.VariantSku,
            Barcode = x.Barcode,
            TrackingPolicy = x.TrackingPolicy.ToString(),
            BaseUomRef = x.BaseUomRef,
            IsActive = x.IsActive,
            InventoryMovementLocked = x.InventoryMovementLocked
        };

    private static VariantAttributeValueViewItem ToVariantAttributeValueItem(VariantAttributeValueReadModel x)
        => new()
        {
            VariantAttributeValueBusinessKey = x.BusinessKey,
            VariantRef = x.VariantRef,
            AttributeRef = x.AttributeRef,
            Value = x.Value,
            OptionRef = x.OptionRef
        };

    private static VariantUomConversionViewItem ToVariantUomConversionItem(VariantUomConversionReadModel x)
        => new()
        {
            VariantUomConversionBusinessKey = x.BusinessKey,
            VariantRef = x.VariantRef,
            FromUomRef = x.FromUomRef,
            ToUomRef = x.ToUomRef,
            Factor = x.Factor,
            RoundingMode = x.RoundingMode.ToString(),
            IsBasePath = x.IsBasePath
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

    private sealed class VariantProductCategoryContext
    {
        public Guid VariantRef { get; set; }
        public Guid ProductRef { get; set; }
        public Guid CategoryRef { get; set; }
        public Guid CategorySchemaVersionRef { get; set; }
    }

    private sealed record CategoryLineageNode(Guid BusinessKey, int Depth);
}

