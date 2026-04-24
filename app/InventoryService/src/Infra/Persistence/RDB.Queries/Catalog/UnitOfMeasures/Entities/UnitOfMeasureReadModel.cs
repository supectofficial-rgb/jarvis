namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Entities;

public class UnitOfMeasureReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Precision { get; set; }
    public bool IsActive { get; set; }
}
