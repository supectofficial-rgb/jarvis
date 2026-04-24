namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;

public class GetCategoryByBusinessKeyQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid? CurrentCategorySchemaVersionRef { get; set; }
    public Guid? ParentCategoryRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryAttributeRuleResultItem> AttributeRules { get; set; } = new();
}

public class CategoryAttributeRuleResultItem
{
    public Guid CategorySchemaVersionRef { get; set; }
    public Guid AttributeRef { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; }
}
