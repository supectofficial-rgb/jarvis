namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Entities;

public class TagReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagColor { get; set; }
}
