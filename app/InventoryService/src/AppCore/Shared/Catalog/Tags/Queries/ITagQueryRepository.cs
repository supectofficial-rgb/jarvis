namespace Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Queries;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;
using OysterFx.AppCore.Shared.Queries;

public interface ITagQueryRepository : IQueryRepository
{
    Task<List<VariantTagLookupItem>> GetVariantTagLookupAsync(string? term = null, int take = 50);
}
