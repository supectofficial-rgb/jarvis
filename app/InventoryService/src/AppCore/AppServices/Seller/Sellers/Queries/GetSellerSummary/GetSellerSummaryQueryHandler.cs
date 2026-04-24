namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Queries.GetSellerSummary;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerSummaryQueryHandler : QueryHandler<GetSellerSummaryQuery, GetSellerSummaryQueryResult>
{
    private readonly ISellerQueryRepository _repository;

    public GetSellerSummaryQueryHandler(ISellerQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerSummaryQueryResult>> ExecuteAsync(GetSellerSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.SellerBusinessKey);
        if (item is null)
            return QueryResult<GetSellerSummaryQueryResult>.Fail("Seller was not found.", "NOT_FOUND");

        return QueryResult<GetSellerSummaryQueryResult>.Success(new GetSellerSummaryQueryResult
        {
            Item = item
        });
    }
}
