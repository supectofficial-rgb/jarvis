namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeleteCategory;

using OysterFx.AppCore.Shared.Commands;

public class DeleteCategoryCommand : ICommand<DeleteCategoryCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
}
