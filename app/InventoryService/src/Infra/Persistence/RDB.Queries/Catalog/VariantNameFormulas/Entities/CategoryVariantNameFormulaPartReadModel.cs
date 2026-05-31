namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;

public class CategoryVariantNameFormulaPartReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid FormulaRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string Separator { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
