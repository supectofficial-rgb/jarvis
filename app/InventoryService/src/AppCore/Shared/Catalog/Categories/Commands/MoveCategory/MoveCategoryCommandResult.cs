namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.MoveCategory;

public class MoveCategoryCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid? ParentCategoryRef { get; set; }
}
