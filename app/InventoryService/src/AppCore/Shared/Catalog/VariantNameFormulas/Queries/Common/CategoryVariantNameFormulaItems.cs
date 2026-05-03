namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.Common;

public class CategoryVariantNameFormulaItem
{
    public Guid FormulaBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Separator { get; set; } = " ";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string Preview { get; set; } = string.Empty;
    public List<CategoryVariantNameFormulaPartItem> Parts { get; set; } = new();
}

public class CategoryVariantNameFormulaPartItem
{
    public Guid PartBusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
