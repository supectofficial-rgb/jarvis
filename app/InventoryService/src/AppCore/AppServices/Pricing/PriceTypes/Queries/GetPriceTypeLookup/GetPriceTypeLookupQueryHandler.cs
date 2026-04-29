namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Queries.GetPriceTypeLookup;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetPriceTypeLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetPriceTypeLookupQueryHandler : QueryHandler<GetPriceTypeLookupQuery, GetPriceTypeLookupQueryResult>
{
    private readonly IPriceTypeQueryRepository _repository;

    public GetPriceTypeLookupQueryHandler(IPriceTypeQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetPriceTypeLookupQueryResult>> ExecuteAsync(GetPriceTypeLookupQuery request)
        => QueryResult<GetPriceTypeLookupQueryResult>.Success(new GetPriceTypeLookupQueryResult { Items = await _repository.GetLookupAsync(request.IncludeInactive) });
}
