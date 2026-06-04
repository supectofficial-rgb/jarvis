namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IStockDetailCommandRepository : ICommandRepository<StockDetail, long>
{
    Task<StockDetail?> GetByBusinessKeyAsync(Guid stockDetailBusinessKey);
    Task<bool> ExistsByVariantRefAsync(Guid variantRef, bool onlyActive = true);

    Task<StockDetail?> FindByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo);

    Task<StockDetail?> FindExactByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo);
}
