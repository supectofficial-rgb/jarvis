namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ILocationStructureCommandRepository : ICommandRepository<LocationStructureNode, long>
{
    Task InsertAsync(LocationStructureValue entity);
    Task<LocationStructureNode?> GetNodeByBusinessKeyAsync(Guid structureBusinessKey);
    Task<LocationStructureValue?> GetValueByBusinessKeyAsync(Guid valueBusinessKey);
    Task<bool> ExistsNodeCodeAsync(Guid warehouseRef, string code, Guid? excludedBusinessKey = null);
    Task<bool> ExistsValueCodeAsync(Guid structureRef, string code, Guid? excludedBusinessKey = null);
}
