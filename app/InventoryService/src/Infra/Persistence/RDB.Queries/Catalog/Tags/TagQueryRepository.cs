namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Queries;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class TagQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ITagQueryRepository
{
    public TagQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<VariantTagLookupItem>> GetVariantTagLookupAsync(string? term = null, int take = 50)
    {
        var query = _dbContext.Set<TagReadModel>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var normalized = term.Trim();
            query = query.Where(x => EF.Functions.ILike(x.TagName, $"%{normalized}%"));
        }

        return await query
            .GroupJoin(
                _dbContext.Set<VariantTagReadModel>().AsNoTracking(),
                tag => tag.BusinessKey,
                variantTag => variantTag.TagRef,
                (tag, variantTags) => new VariantTagLookupItem
                {
                    TagId = tag.BusinessKey,
                    TagName = tag.TagName,
                    TagColor = tag.TagColor,
                    UsageCount = variantTags.Count()
                })
            .OrderBy(x => x.TagName)
            .Take(take <= 0 ? 50 : take)
            .ToListAsync();
    }
}
