namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategory;

using OysterFx.AppCore.Shared.Commands;

public class ActivateCategoryCommand : ICommand<ActivateCategoryCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
}
