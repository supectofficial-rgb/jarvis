namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UnsetSellerAsSystemOwner;

using OysterFx.AppCore.Shared.Commands;

public class UnsetSellerAsSystemOwnerCommand : ICommand<UnsetSellerAsSystemOwnerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
}
