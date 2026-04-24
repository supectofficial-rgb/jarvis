namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeleteSeller;

public class DeleteSellerCommandResult
{
    public Guid SellerBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
