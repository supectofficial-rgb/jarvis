namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;

public sealed class LocationStructureSelectionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid LocationRef { get; set; }
    public Guid StructureRef { get; set; }
    public Guid StructureValueRef { get; set; }
}
