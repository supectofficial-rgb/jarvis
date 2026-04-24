namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.Common;

public class CategoryListItem
{
    public Guid CategoryBusinessKey { get; set; }
    public Guid? ParentCategoryRef { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryTreeItem : CategoryListItem
{
    public List<CategoryTreeItem> Children { get; set; } = new();
}

public class CategoryBreadcrumbItem
{
    public Guid CategoryBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Depth { get; set; }
}

public class CategorySummaryItem
{
    public Guid CategoryBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public int ActiveProductCount { get; set; }
    public int VariantCount { get; set; }
    public int ActiveVariantCount { get; set; }
}

public class CategoryAttributeRuleViewItem
{
    public Guid RuleBusinessKey { get; set; }
    public Guid CategoryBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
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
    public List<CategoryAttributeRuleOptionViewItem> Options { get; set; } = new();
}

public class CategoryAttributeRuleOptionViewItem
{
    public Guid OptionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryAttributeFormDefinitionItem
{
    public List<CategoryAttributeRuleViewItem> ProductLevelAttributes { get; set; } = new();
    public List<CategoryAttributeRuleViewItem> VariantLevelAttributes { get; set; } = new();
}
