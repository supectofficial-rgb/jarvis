namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;

public class CategorySchemaVersionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public int VersionNo { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ChangeSummary { get; set; }
}
