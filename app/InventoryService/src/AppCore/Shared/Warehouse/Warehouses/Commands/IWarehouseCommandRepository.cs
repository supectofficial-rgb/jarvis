namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IWarehouseCommandRepository : ICommandRepository<Warehouse, long>
{
    Task<Warehouse?> GetByBusinessKeyAsync(Guid warehouseBusinessKey);

    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}
