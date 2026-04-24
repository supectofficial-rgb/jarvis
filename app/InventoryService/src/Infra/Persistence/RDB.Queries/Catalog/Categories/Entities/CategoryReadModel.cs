namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;

public class CategoryReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid? ParentCategoryRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
