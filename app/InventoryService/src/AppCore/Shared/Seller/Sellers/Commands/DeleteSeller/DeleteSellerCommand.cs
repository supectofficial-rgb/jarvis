namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.DeleteSeller;

using OysterFx.AppCore.Shared.Commands;

public class DeleteSellerCommand : ICommand<DeleteSellerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
}
