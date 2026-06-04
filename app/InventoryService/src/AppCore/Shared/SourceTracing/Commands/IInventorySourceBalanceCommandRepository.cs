namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventorySourceBalanceCommandRepository : ICommandRepository<InventorySourceBalance, long>
{
    Task<InventorySourceBalance?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey);
    Task<List<InventorySourceBalance>> GetByReservationRefAsync(Guid reservationRef);
    Task<List<InventorySourceBalance>> GetOpenByPoolAsync(
        Guid variantRef,
        Guid warehouseRef,
        Guid? qualityStatusRef = null,
        string? lotBatchNo = null);
}
