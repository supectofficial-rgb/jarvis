namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategory;

public class ActivateCategoryCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
