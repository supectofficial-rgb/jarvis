namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;

public class GetSellerByBusinessKeyQueryResult
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; }
}
