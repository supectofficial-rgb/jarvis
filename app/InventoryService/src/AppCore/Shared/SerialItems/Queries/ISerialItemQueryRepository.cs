namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;
using OysterFx.AppCore.Shared.Queries;

public interface ISerialItemQueryRepository : IQueryRepository
{
    Task<GetSerialItemByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid serialItemBusinessKey);
    Task<SerialItemListItem?> GetByIdAsync(Guid serialItemId);
    Task<SerialItemListItem?> GetBySerialNoAsync(string serialNo, Guid? variantRef = null);
    Task<SearchSerialItemsQueryResult> SearchAsync(SearchSerialItemsQuery query);
    Task<List<SerialItemListItem>> GetByVariantAsync(Guid variantRef);
    Task<List<SerialItemListItem>> GetAvailableAsync(Guid? variantRef = null, Guid? warehouseRef = null);
}
