namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetPriceChannelByBusinessKeyQueryHandler : QueryHandler<GetPriceChannelByBusinessKeyQuery, GetPriceChannelByBusinessKeyQueryResult>
{
    private readonly IPriceChannelQueryRepository _repository;

    public GetPriceChannelByBusinessKeyQueryHandler(IPriceChannelQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetPriceChannelByBusinessKeyQueryResult>> ExecuteAsync(GetPriceChannelByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.PriceChannelBusinessKey);
        return item is null
            ? QueryResult<GetPriceChannelByBusinessKeyQueryResult>.Fail("Price channel was not found.", "NOT_FOUND")
            : QueryResult<GetPriceChannelByBusinessKeyQueryResult>.Success(item);
    }
}
