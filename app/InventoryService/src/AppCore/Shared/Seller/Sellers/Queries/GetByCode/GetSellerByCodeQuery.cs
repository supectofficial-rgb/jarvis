namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByCode;

using OysterFx.AppCore.Shared.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;

public class GetSellerByCodeQuery : IQuery<GetSellerByBusinessKeyQueryResult>
{
    public GetSellerByCodeQuery(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
