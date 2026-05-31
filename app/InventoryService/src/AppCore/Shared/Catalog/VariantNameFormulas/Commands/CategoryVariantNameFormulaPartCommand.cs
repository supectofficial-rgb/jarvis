namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;

public sealed class CategoryVariantNameFormulaPartCommand
{
    public Guid AttributeRef { get; set; }
    public string? Separator { get; set; }
    public int SortOrder { get; set; }
}
