namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
