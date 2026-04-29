namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;

using OysterFx.AppCore.Shared.Queries;

public class SearchPriceTypesQuery : IQuery<SearchPriceTypesQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
