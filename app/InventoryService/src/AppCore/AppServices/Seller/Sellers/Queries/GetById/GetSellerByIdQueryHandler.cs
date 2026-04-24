namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerByIdQueryHandler : QueryHandler<GetSellerByIdQuery, GetSellerByBusinessKeyQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetSellerByIdQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerByBusinessKeyQueryResult>> ExecuteAsync(GetSellerByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.SellerId);
        if (item is null)
            return QueryResult<GetSellerByBusinessKeyQueryResult>.Fail("Seller was not found.", "NOT_FOUND");

        return QueryResult<GetSellerByBusinessKeyQueryResult>.Success(item);
    }
}
