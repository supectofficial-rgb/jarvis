namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UnsetSellerAsSystemOwner;

public class UnsetSellerAsSystemOwnerCommandResult
{
    public Guid SellerBusinessKey { get; set; }
    public bool IsSystemOwner { get; set; }
}
