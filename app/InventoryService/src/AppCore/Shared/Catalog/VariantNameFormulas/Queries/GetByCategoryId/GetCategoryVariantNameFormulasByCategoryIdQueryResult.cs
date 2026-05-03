namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.GetByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.Common;

public class GetCategoryVariantNameFormulasByCategoryIdQueryResult
{
    public Guid CategoryRef { get; set; }
    public List<CategoryVariantNameFormulaItem> Items { get; set; } = new();
}
