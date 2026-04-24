namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetSellerLookup;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerLookupQueryHandler : QueryHandler<GetSellerLookupQuery, GetSellerLookupQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetSellerLookupQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerLookupQueryResult>> ExecuteAsync(GetSellerLookupQuery request)
    {
        var items = await _repository.GetLookupAsync(request.IncludeInactive);
        return QueryResult<GetSellerLookupQueryResult>.Success(new GetSellerLookupQueryResult
        {
            Items = items
        });
    }
}
