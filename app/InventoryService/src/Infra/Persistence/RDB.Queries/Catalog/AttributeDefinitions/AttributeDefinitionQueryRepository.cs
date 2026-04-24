namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class AttributeDefinitionQueryRepository
    : QueryRepository<InventoryServiceQueryDbContext>, IAttributeDefinitionQueryRepository
{
    public AttributeDefinitionQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<GetAttributeDefinitionByBusinessKeyQueryResult?> GetByIdAsync(Guid attributeDefinitionId)
        => GetByBusinessKeyAsync(attributeDefinitionId);

    public async Task<GetAttributeDefinitionByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid attributeDefinitionBusinessKey)
    {
        var aggregate = await _dbContext.Set<AttributeDefinitionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == attributeDefinitionBusinessKey);

        if (aggregate is null)
            return null;

        var options = await _dbContext.Set<AttributeOptionReadModel>()
            .Where(x => x.AttributeRef == attributeDefinitionBusinessKey)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new AttributeOptionResultItem
            {
                Name = string.IsNullOrWhiteSpace(x.Name) ? x.Value : x.Name,
                Value = x.Value,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new GetAttributeDefinitionByBusinessKeyQueryResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey,
            Code = aggregate.Code,
            Name = aggregate.Name,
            DataType = aggregate.DataType.ToString(),
            Scope = aggregate.Scope.ToString(),
            IsActive = aggregate.IsActive,
            Options = options
        };
    }

    public async Task<GetAttributeDefinitionListQueryResult> GetListAsync(GetAttributeDefinitionListQuery query)
    {
        var searchResult = await SearchAsync(new SearchAttributeDefinitionsQuery
        {
            Code = query.Code,
            Name = query.Name,
            DataType = query.DataType,
            Scope = query.Scope,
            IsActive = query.IsActive,
            Page = query.Page,
            PageSize = query.PageSize
        });

        return new GetAttributeDefinitionListQueryResult
        {
            TotalCount = searchResult.TotalCount,
            Page = searchResult.Page,
            PageSize = searchResult.PageSize,
            Items = searchResult.Items.Select(x => new GetAttributeDefinitionListItem
            {
                AttributeDefinitionBusinessKey = x.AttributeDefinitionBusinessKey,
                Code = x.Code,
                Name = x.Name,
                DataType = x.DataType,
                Scope = x.Scope,
                IsActive = x.IsActive,
                OptionCount = x.OptionCount
            }).ToList()
        };
    }

    public async Task<SearchAttributeDefinitionsQueryResult> SearchAsync(SearchAttributeDefinitionsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<AttributeDefinitionReadModel> dbQuery = _dbContext.Set<AttributeDefinitionReadModel>();

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

        if (!string.IsNullOrWhiteSpace(query.DataType) && Enum.TryParse<AttributeDataType>(query.DataType, true, out var dataType))
            dbQuery = dbQuery.Where(x => x.DataType == dataType);

        if (!string.IsNullOrWhiteSpace(query.Scope) && Enum.TryParse<AttributeScope>(query.Scope, true, out var scope))
            dbQuery = dbQuery.Where(x => x.Scope == scope);

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await dbQuery.CountAsync();

        var aggregates = await dbQuery
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var keys = aggregates.Select(x => x.BusinessKey).ToList();
        var optionCounts = keys.Count == 0
            ? new Dictionary<Guid, int>()
            : await _dbContext.Set<AttributeOptionReadModel>()
                .Where(x => keys.Contains(x.AttributeRef) && x.IsActive)
                .GroupBy(x => x.AttributeRef)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

        return new SearchAttributeDefinitionsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = aggregates.Select(x => ToListItem(x, optionCounts.TryGetValue(x.BusinessKey, out var count) ? count : 0)).ToList()
        };
    }

    public async Task<List<AttributeDefinitionListItem>> GetActiveAsync()
    {
        var definitions = await _dbContext.Set<AttributeDefinitionReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .ToListAsync();

        return await MapToListItemsAsync(definitions);
    }

    public async Task<List<AttributeDefinitionListItem>> GetByScopeAsync(string scope, bool includeInactive = false)
    {
        IQueryable<AttributeDefinitionReadModel> query = _dbContext.Set<AttributeDefinitionReadModel>();

        if (Enum.TryParse<AttributeScope>(scope, true, out var parsedScope))
            query = query.Where(x => x.Scope == parsedScope);
        else
            return new List<AttributeDefinitionListItem>();

        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        var definitions = await query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Code)
            .ToListAsync();

        return await MapToListItemsAsync(definitions);
    }

    public async Task<AttributeDefinitionSummaryItem?> GetSummaryAsync(Guid attributeDefinitionId)
    {
        var definition = await _dbContext.Set<AttributeDefinitionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == attributeDefinitionId);

        if (definition is null)
            return null;

        var optionCount = await _dbContext.Set<AttributeOptionReadModel>()
            .CountAsync(x => x.AttributeRef == attributeDefinitionId && x.IsActive);

        var categoryRuleUsageCount = await _dbContext.Set<CategoryAttributeRuleReadModel>()
            .CountAsync(x => x.AttributeRef == attributeDefinitionId && x.IsActive);

        var productValueUsageCount = await _dbContext.Set<ProductAttributeValueReadModel>()
            .CountAsync(x => x.AttributeRef == attributeDefinitionId);

        var variantValueUsageCount = await _dbContext.Set<VariantAttributeValueReadModel>()
            .CountAsync(x => x.AttributeRef == attributeDefinitionId);

        return new AttributeDefinitionSummaryItem
        {
            AttributeDefinitionBusinessKey = definition.BusinessKey,
            Code = definition.Code,
            Name = definition.Name,
            DataType = definition.DataType.ToString(),
            Scope = definition.Scope.ToString(),
            IsActive = definition.IsActive,
            OptionCount = optionCount,
            CategoryRuleUsageCount = categoryRuleUsageCount,
            ProductValueUsageCount = productValueUsageCount,
            VariantValueUsageCount = variantValueUsageCount
        };
    }

    public async Task<AttributeOptionListItem?> GetOptionByIdAsync(Guid optionId)
    {
        var option = await _dbContext.Set<AttributeOptionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == optionId);

        return option is null ? null : ToOptionItem(option);
    }

    public async Task<List<AttributeOptionListItem>> GetOptionsByAttributeIdAsync(Guid attributeDefinitionId, bool includeInactive = true)
    {
        var query = _dbContext.Set<AttributeOptionReadModel>()
            .Where(x => x.AttributeRef == attributeDefinitionId);

        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => ToOptionItem(x))
            .ToListAsync();
    }

    public async Task<SearchAttributeOptionsQueryResult> SearchOptionsAsync(SearchAttributeOptionsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<AttributeOptionReadModel> dbQuery = _dbContext.Set<AttributeOptionReadModel>();

        if (query.AttributeDefinitionId.HasValue)
            dbQuery = dbQuery.Where(x => x.AttributeRef == query.AttributeDefinitionId.Value);

        if (!string.IsNullOrWhiteSpace(query.Value))
        {
            var value = query.Value.Trim();
            dbQuery = dbQuery.Where(x =>
                EF.Functions.ILike(x.Value, $"%{value}%")
                || EF.Functions.ILike(x.Name, $"%{value}%"));
        }

        if (query.IsActive.HasValue)
            dbQuery = dbQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await dbQuery.CountAsync();

        var items = await dbQuery
            .OrderBy(x => x.AttributeRef)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToOptionItem(x))
            .ToListAsync();

        return new SearchAttributeOptionsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    private async Task<List<AttributeDefinitionListItem>> MapToListItemsAsync(List<AttributeDefinitionReadModel> definitions)
    {
        var keys = definitions.Select(x => x.BusinessKey).ToList();
        var optionCounts = keys.Count == 0
            ? new Dictionary<Guid, int>()
            : await _dbContext.Set<AttributeOptionReadModel>()
                .Where(x => keys.Contains(x.AttributeRef) && x.IsActive)
                .GroupBy(x => x.AttributeRef)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

        return definitions
            .Select(x => ToListItem(x, optionCounts.TryGetValue(x.BusinessKey, out var count) ? count : 0))
            .ToList();
    }

    private static AttributeDefinitionListItem ToListItem(AttributeDefinitionReadModel x, int optionCount)
        => new()
        {
            AttributeDefinitionBusinessKey = x.BusinessKey,
            Code = x.Code,
            Name = x.Name,
            DataType = x.DataType.ToString(),
            Scope = x.Scope.ToString(),
            IsActive = x.IsActive,
            OptionCount = optionCount
        };

    private static AttributeOptionListItem ToOptionItem(AttributeOptionReadModel x)
        => new()
        {
            OptionBusinessKey = x.BusinessKey,
            AttributeDefinitionBusinessKey = x.AttributeRef,
            Name = string.IsNullOrWhiteSpace(x.Name) ? x.Value : x.Name,
            Value = x.Value,
            DisplayOrder = x.DisplayOrder,
            IsActive = x.IsActive
        };
}
