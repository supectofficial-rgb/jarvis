namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.CreateCategoryVariantNameFormula;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateCategoryVariantNameFormulaCommand : ICommand<CreateCategoryVariantNameFormulaCommandResult>
{
    public Guid CategoryRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Separator { get; set; } = " ";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> AttributeRefs { get; set; } = new();
    public List<CategoryVariantNameFormulaPartCommand> Parts { get; set; } = new();
}
