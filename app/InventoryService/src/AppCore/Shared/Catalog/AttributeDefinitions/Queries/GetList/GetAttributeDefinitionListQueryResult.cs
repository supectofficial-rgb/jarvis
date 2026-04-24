namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;

public class GetAttributeDefinitionListQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<GetAttributeDefinitionListItem> Items { get; set; } = new();
}

public class GetAttributeDefinitionListItem
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int OptionCount { get; set; }
}
