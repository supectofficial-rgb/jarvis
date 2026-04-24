namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ILocationCommandRepository : ICommandRepository<Location, long>
{
    Task<Location?> GetByBusinessKeyAsync(Guid locationBusinessKey);

    Task<bool> ExistsByCodeAsync(string locationCode, Guid? exceptBusinessKey = null);
}
