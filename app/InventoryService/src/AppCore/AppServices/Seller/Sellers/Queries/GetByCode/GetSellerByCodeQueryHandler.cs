namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByCode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerByCodeQueryHandler : QueryHandler<GetSellerByCodeQuery, GetSellerByBusinessKeyQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetSellerByCodeQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerByBusinessKeyQueryResult>> ExecuteAsync(GetSellerByCodeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return QueryResult<GetSellerByBusinessKeyQueryResult>.Fail("Code is required.", "VALIDATION");

        var item = await _repository.GetByCodeAsync(request.Code);
        if (item is null)
            return QueryResult<GetSellerByBusinessKeyQueryResult>.Fail("Seller was not found.", "NOT_FOUND");

        return QueryResult<GetSellerByBusinessKeyQueryResult>.Success(item);
    }
}
