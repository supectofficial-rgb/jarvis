namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Queries.GetPriceChannelLookup;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetPriceChannelLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetPriceChannelLookupQueryHandler : QueryHandler<GetPriceChannelLookupQuery, GetPriceChannelLookupQueryResult>
{
    private readonly IPriceChannelQueryRepository _repository;

    public GetPriceChannelLookupQueryHandler(IPriceChannelQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetPriceChannelLookupQueryResult>> ExecuteAsync(GetPriceChannelLookupQuery request)
        => QueryResult<GetPriceChannelLookupQueryResult>.Success(new GetPriceChannelLookupQueryResult { Items = await _repository.GetLookupAsync(request.IncludeInactive) });
}
