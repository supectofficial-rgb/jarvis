namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.AttributeDefinitions;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class AttributeDefinitionCommandRepository
    : CommandRepository<AttributeDefinition, InventoryServiceCommandDbContext>, IAttributeDefinitionCommandRepository
{
    public AttributeDefinitionCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<AttributeDefinition?> GetByBusinessKeyAsync(Guid attributeDefinitionBusinessKey)
    {
        var businessKey = BusinessKey.FromGuid(attributeDefinitionBusinessKey);
        return _dbContext.Set<AttributeDefinition>()
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.BusinessKey == businessKey);
    }

    public async Task<IReadOnlyCollection<AttributeDefinition>> GetByBusinessKeysAsync(IReadOnlyCollection<Guid> attributeDefinitionBusinessKeys)
    {
        if (attributeDefinitionBusinessKeys is null || attributeDefinitionBusinessKeys.Count == 0)
            return Array.Empty<AttributeDefinition>();

        var keys = attributeDefinitionBusinessKeys
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (keys.Count == 0)
            return Array.Empty<AttributeDefinition>();

        var businessKeys = keys.Select(BusinessKey.FromGuid).ToList();

        return await _dbContext.Set<AttributeDefinition>()
            .Include(x => x.Options)
            .Where(x => businessKeys.Contains(x.BusinessKey))
            .ToListAsync();
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalizedCode = code.Trim();

        var query = _dbContext.Set<AttributeDefinition>()
            .Where(x => x.Code == normalizedCode);

        if (exceptBusinessKey.HasValue)
        {
            var exceptKey = BusinessKey.FromGuid(exceptBusinessKey.Value);
            query = query.Where(x => x.BusinessKey != exceptKey);
        }

        return query.AnyAsync();
    }
}
