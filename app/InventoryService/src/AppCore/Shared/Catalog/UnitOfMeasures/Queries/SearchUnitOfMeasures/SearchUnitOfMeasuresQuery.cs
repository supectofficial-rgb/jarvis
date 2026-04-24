namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;

using OysterFx.AppCore.Shared.Queries;

public class SearchUnitOfMeasuresQuery : IQuery<SearchUnitOfMeasuresQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
