namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class AttributeDefinitionListItem
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OptionCount { get; set; }
}

public class AttributeOptionListItem
{
    public Guid OptionBusinessKey { get; set; }
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class AttributeDefinitionSummaryItem
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OptionCount { get; set; }
    public int CategoryRuleUsageCount { get; set; }
    public int ProductValueUsageCount { get; set; }
    public int VariantValueUsageCount { get; set; }
}
