namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Tags;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class TagCommandRepository : CommandRepository<Tag, InventoryServiceCommandDbContext>, ITagCommandRepository
{
    public TagCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Tag?> GetByBusinessKeyAsync(Guid tagBusinessKey)
    {
        return _dbContext.Set<Tag>()
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(tagBusinessKey));
    }

    public Task<bool> ExistsByNameAsync(string tagName, Guid? exceptBusinessKey = null)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            return Task.FromResult(false);

        var normalized = tagName.Trim();
        var query = _dbContext.Set<Tag>().Where(x => x.TagName == normalized);

        if (exceptBusinessKey.HasValue && exceptBusinessKey.Value != Guid.Empty)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
