namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Entities;

public class WarehouseReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
