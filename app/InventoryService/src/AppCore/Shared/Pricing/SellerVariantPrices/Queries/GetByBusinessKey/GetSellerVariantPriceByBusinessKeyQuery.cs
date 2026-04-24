namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetSellerVariantPriceByBusinessKeyQuery : IQuery<GetSellerVariantPriceByBusinessKeyQueryResult>
{
    public GetSellerVariantPriceByBusinessKeyQuery(Guid sellerVariantPriceBusinessKey)
    {
        SellerVariantPriceBusinessKey = sellerVariantPriceBusinessKey;
    }

    public Guid SellerVariantPriceBusinessKey { get; }
}
