namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeactivateSeller;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateSellerCommand : ICommand<DeactivateSellerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
}
