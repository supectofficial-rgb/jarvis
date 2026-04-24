namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.SetSellerAsSystemOwner;

using OysterFx.AppCore.Shared.Commands;

public class SetSellerAsSystemOwnerCommand : ICommand<SetSellerAsSystemOwnerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
    public bool IsSystemOwner { get; set; } = true;
}
