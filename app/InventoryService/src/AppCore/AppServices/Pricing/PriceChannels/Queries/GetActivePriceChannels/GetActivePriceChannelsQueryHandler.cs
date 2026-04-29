namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Queries.GetActivePriceChannels;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetActivePriceChannels;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActivePriceChannelsQueryHandler : QueryHandler<GetActivePriceChannelsQuery, GetActivePriceChannelsQueryResult>
{
    private readonly IPriceChannelQueryRepository _repository;

    public GetActivePriceChannelsQueryHandler(IPriceChannelQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActivePriceChannelsQueryResult>> ExecuteAsync(GetActivePriceChannelsQuery request)
        => QueryResult<GetActivePriceChannelsQueryResult>.Success(new GetActivePriceChannelsQueryResult { Items = await _repository.GetActiveAsync() });
}
