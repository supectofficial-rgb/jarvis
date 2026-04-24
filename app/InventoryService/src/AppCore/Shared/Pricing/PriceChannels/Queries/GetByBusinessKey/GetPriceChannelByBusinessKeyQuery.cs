namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetPriceChannelByBusinessKeyQuery : IQuery<GetPriceChannelByBusinessKeyQueryResult>
{
    public GetPriceChannelByBusinessKeyQuery(Guid priceChannelBusinessKey)
    {
        PriceChannelBusinessKey = priceChannelBusinessKey;
    }

    public Guid PriceChannelBusinessKey { get; }
}
