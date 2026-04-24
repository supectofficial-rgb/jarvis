namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;

using OysterFx.AppCore.Shared.Queries;

public class SearchAttributeOptionsQuery : IQuery<SearchAttributeOptionsQueryResult>
{
    public Guid? AttributeDefinitionId { get; set; }
    public string? Value { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
