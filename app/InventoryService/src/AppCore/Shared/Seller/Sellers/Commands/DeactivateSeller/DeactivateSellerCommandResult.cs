namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeactivateSeller;

public class DeactivateSellerCommandResult
{
    public Guid SellerBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
