namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.UpdateCategoryVariantNameFormula;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using OysterFx.AppCore.Shared.Commands;

public class UpdateCategoryVariantNameFormulaCommand : ICommand<UpdateCategoryVariantNameFormulaCommandResult>
{
    public Guid FormulaBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Separator { get; set; } = " ";
    public bool IncludeCategoryName { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> AttributeRefs { get; set; } = new();
    public List<CategoryVariantNameFormulaPartCommand> Parts { get; set; } = new();
}
