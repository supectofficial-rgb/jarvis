namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Queries.SearchPriceTypes;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchPriceTypesQueryHandler : QueryHandler<SearchPriceTypesQuery, SearchPriceTypesQueryResult>
{
    private readonly IPriceTypeQueryRepository _repository;

    public SearchPriceTypesQueryHandler(IPriceTypeQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchPriceTypesQueryResult>> ExecuteAsync(SearchPriceTypesQuery request)
        => QueryResult<SearchPriceTypesQueryResult>.Success(await _repository.SearchAsync(request));
}
