namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventorySourceBalanceCommandRepository : ICommandRepository<InventorySourceBalance, long>
{
    Task<InventorySourceBalance?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey);
    Task<List<InventorySourceBalance>> GetOpenByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        Guid? serialRef);
}
