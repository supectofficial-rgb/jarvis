namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategory;

public class DeactivateCategoryCommandResult
{
    public Guid CategoryBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
