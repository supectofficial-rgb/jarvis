namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.UpdateCategoryVariantNameFormula;

using OysterFx.AppCore.Shared.Commands;

public class UpdateCategoryVariantNameFormulaCommand : ICommand<UpdateCategoryVariantNameFormulaCommandResult>
{
    public Guid FormulaBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Separator { get; set; } = " ";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> AttributeRefs { get; set; } = new();
}
