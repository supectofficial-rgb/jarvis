namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ISerialItemCommandRepository : ICommandRepository<SerialItem, long>
{
    Task<SerialItem?> GetByBusinessKeyAsync(Guid serialItemBusinessKey);
    Task<bool> ExistsBySerialNoAsync(Guid variantRef, string serialNo, Guid? exceptBusinessKey = null);
}
