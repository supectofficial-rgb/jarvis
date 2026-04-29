namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;

using OysterFx.AppCore.Shared.Queries;

public class SearchPriceChannelsQuery : IQuery<SearchPriceChannelsQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
