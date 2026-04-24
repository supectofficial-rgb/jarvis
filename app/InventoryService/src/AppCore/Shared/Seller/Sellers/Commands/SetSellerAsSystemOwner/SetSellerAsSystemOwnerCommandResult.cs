namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.SetSellerAsSystemOwner;

public class SetSellerAsSystemOwnerCommandResult
{
    public Guid SellerBusinessKey { get; set; }
    public bool IsSystemOwner { get; set; }
}
