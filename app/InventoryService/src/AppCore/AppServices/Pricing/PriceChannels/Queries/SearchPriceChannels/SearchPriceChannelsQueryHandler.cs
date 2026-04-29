namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Queries.SearchPriceChannels;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchPriceChannelsQueryHandler : QueryHandler<SearchPriceChannelsQuery, SearchPriceChannelsQueryResult>
{
    private readonly IPriceChannelQueryRepository _repository;

    public SearchPriceChannelsQueryHandler(IPriceChannelQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchPriceChannelsQueryResult>> ExecuteAsync(SearchPriceChannelsQuery request)
        => QueryResult<SearchPriceChannelsQueryResult>.Success(await _repository.SearchAsync(request));
}
