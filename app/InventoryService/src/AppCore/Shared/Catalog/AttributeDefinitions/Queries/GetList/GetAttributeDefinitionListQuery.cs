namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeDefinitionListQuery : IQuery<GetAttributeDefinitionListQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? DataType { get; set; }
    public string? Scope { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
