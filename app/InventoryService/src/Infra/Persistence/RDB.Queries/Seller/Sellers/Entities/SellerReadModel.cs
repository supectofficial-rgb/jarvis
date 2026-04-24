namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Entities;

public class SellerReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; }
}
