namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;

public class CategoryVariantNameFormulaReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Separator { get; set; } = " ";
    public bool IncludeCategoryName { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
