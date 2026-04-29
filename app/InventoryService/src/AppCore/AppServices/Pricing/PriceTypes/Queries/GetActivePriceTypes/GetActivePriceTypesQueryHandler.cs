namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Queries.GetActivePriceTypes;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetActivePriceTypes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActivePriceTypesQueryHandler : QueryHandler<GetActivePriceTypesQuery, GetActivePriceTypesQueryResult>
{
    private readonly IPriceTypeQueryRepository _repository;

    public GetActivePriceTypesQueryHandler(IPriceTypeQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActivePriceTypesQueryResult>> ExecuteAsync(GetActivePriceTypesQuery request)
        => QueryResult<GetActivePriceTypesQueryResult>.Success(new GetActivePriceTypesQueryResult { Items = await _repository.GetActiveAsync() });
}
