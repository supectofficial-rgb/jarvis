namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetPriceTypeByBusinessKeyQueryHandler : QueryHandler<GetPriceTypeByBusinessKeyQuery, GetPriceTypeByBusinessKeyQueryResult>
{
    private readonly IPriceTypeQueryRepository _repository;

    public GetPriceTypeByBusinessKeyQueryHandler(IPriceTypeQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetPriceTypeByBusinessKeyQueryResult>> ExecuteAsync(GetPriceTypeByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.PriceTypeBusinessKey);
        return item is null
            ? QueryResult<GetPriceTypeByBusinessKeyQueryResult>.Fail("Price type was not found.", "NOT_FOUND")
            : QueryResult<GetPriceTypeByBusinessKeyQueryResult>.Success(item);
    }
}
