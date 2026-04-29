namespace Insurance.InventoryService.AppCore.AppServices.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerVariantPriceByBusinessKeyQueryHandler : QueryHandler<GetSellerVariantPriceByBusinessKeyQuery, GetSellerVariantPriceByBusinessKeyQueryResult>
{
    private readonly ISellerVariantPriceQueryRepository _repository;

    public GetSellerVariantPriceByBusinessKeyQueryHandler(ISellerVariantPriceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerVariantPriceByBusinessKeyQueryResult>> ExecuteAsync(GetSellerVariantPriceByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.SellerVariantPriceBusinessKey);
        return item is null
            ? QueryResult<GetSellerVariantPriceByBusinessKeyQueryResult>.Fail("Seller variant price was not found.", "NOT_FOUND")
            : QueryResult<GetSellerVariantPriceByBusinessKeyQueryResult>.Success(item);
    }
}
