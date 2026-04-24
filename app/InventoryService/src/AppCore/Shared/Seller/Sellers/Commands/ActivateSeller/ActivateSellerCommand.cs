namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.ActivateSeller;

using OysterFx.AppCore.Shared.Commands;

public class ActivateSellerCommand : ICommand<ActivateSellerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
}
