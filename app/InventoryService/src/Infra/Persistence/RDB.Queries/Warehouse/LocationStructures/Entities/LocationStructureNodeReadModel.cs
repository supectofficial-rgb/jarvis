namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;

public class LocationStructureNodeReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid? ParentStructureRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
