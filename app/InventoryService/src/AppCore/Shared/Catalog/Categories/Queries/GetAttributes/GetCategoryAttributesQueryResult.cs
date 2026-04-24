namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetAttributes;

public class GetCategoryAttributesQueryResult
{
    public Guid CategoryBusinessKey { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool IncludeInherited { get; set; }
    public bool IncludeInactive { get; set; }
    public int TotalCount { get; set; }
    public List<GetCategoryAttributeItem> Items { get; set; } = new();
}

public class GetCategoryAttributeItem
{
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool AttributeIsActive { get; set; }
    public bool RuleIsRequired { get; set; }
    public bool RuleIsVariant { get; set; }
    public int RuleDisplayOrder { get; set; }
    public bool RuleIsOverridden { get; set; }
    public bool RuleIsActive { get; set; }
    public bool IsInherited { get; set; }
    public Guid SourceCategoryRef { get; set; }
    public string SourceCategoryCode { get; set; } = string.Empty;
    public string SourceCategoryName { get; set; } = string.Empty;
    public List<GetCategoryAttributeOptionItem> Options { get; set; } = new();
}

public class GetCategoryAttributeOptionItem
{
    public Guid OptionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
