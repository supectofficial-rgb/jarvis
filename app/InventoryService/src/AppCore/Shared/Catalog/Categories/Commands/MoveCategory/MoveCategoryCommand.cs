namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.MoveCategory;

using OysterFx.AppCore.Shared.Commands;

public class MoveCategoryCommand : ICommand<MoveCategoryCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid? ParentCategoryRef { get; set; }
}
