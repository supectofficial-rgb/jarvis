namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.ActivateSeller;

public class ActivateSellerCommandResult
{
    public Guid SellerBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
