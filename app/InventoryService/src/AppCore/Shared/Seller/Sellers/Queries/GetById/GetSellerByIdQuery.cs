namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;

public class GetSellerByIdQuery : IQuery<GetSellerByBusinessKeyQueryResult>
{
    public GetSellerByIdQuery(Guid sellerId)
    {
        SellerId = sellerId;
    }

    public Guid SellerId { get; }
}
