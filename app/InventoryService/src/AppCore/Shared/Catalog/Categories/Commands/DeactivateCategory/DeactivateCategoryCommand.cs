namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategory;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateCategoryCommand : ICommand<DeactivateCategoryCommandResult>
{
    public Guid CategoryBusinessKey { get; set; }
}
