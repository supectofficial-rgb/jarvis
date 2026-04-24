namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerByBusinessKeyQueryHandler : QueryHandler<GetSellerByBusinessKeyQuery, GetSellerByBusinessKeyQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetSellerByBusinessKeyQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerByBusinessKeyQueryResult>> ExecuteAsync(GetSellerByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.SellerBusinessKey);
        if (item is null)
            return QueryResult<GetSellerByBusinessKeyQueryResult>.Fail("Seller was not found.", "NOT_FOUND");

        return QueryResult<GetSellerByBusinessKeyQueryResult>.Success(item);
    }
}
