namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetActiveSellers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetActiveSellers;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveSellersQueryHandler : QueryHandler<GetActiveSellersQuery, GetActiveSellersQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetActiveSellersQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveSellersQueryResult>> ExecuteAsync(GetActiveSellersQuery request)
    {
        var items = await _repository.GetActiveSellersAsync();
        return QueryResult<GetActiveSellersQueryResult>.Success(new GetActiveSellersQueryResult
        {
            Items = items
        });
    }
}
