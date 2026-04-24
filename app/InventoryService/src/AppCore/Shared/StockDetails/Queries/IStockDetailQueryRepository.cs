namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;
using OysterFx.AppCore.Shared.Queries;

public interface IStockDetailQueryRepository : IQueryRepository
{
    Task<GetStockDetailByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid stockDetailBusinessKey);
    Task<StockDetailListItem?> GetByIdAsync(Guid stockDetailId);
    Task<StockDetailListItem?> GetByBucketKeyAsync(Guid variantRef, Guid sellerRef, Guid warehouseRef, Guid locationRef, Guid qualityStatusRef, string? lotBatchNo);
    Task<SearchStockDetailsQueryResult> SearchAsync(SearchStockDetailsQuery query);
    Task<List<StockDetailListItem>> GetByVariantAsync(Guid variantRef);
    Task<List<StockDetailListItem>> GetBySellerAsync(Guid sellerRef);
    Task<List<StockDetailListItem>> GetByWarehouseAsync(Guid warehouseRef);
    Task<List<StockDetailListItem>> GetByLocationAsync(Guid locationRef);
    Task<List<StockDetailListItem>> GetByQualityStatusAsync(Guid qualityStatusRef);
    Task<List<StockDetailListItem>> GetByLotBatchAsync(string lotBatchNo);
    Task<List<StockDetailListItem>> GetAvailableBucketsAsync(GetAvailableStockBucketsQuery query);
    Task<List<StockDetailListItem>> GetEmptyBucketsAsync(Guid? variantRef, Guid? sellerRef, Guid? warehouseRef, Guid? locationRef, Guid? qualityStatusRef);
    Task<VariantStockSummaryItem?> GetVariantSummaryAsync(Guid variantRef);
    Task<WarehouseStockSummaryItem?> GetWarehouseSummaryAsync(Guid warehouseRef);
    Task<SellerStockSummaryItem?> GetSellerSummaryAsync(Guid sellerRef);
    Task<List<StockAgingItem>> GetStockAgingAsync(GetStockAgingQuery query);
    Task<List<StockDetailListItem>> GetLowStockBucketsAsync(GetLowStockBucketsQuery query);
}
