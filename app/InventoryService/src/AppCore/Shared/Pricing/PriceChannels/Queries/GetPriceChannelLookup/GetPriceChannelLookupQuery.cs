namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetPriceChannelLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetPriceChannelLookupQuery : IQuery<GetPriceChannelLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}
