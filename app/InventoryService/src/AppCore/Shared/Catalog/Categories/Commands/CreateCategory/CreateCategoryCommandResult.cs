namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.CreateCategory;

public class CreateCategoryCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
