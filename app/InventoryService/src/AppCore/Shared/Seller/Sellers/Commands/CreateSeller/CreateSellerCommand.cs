namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.CreateSeller;

using OysterFx.AppCore.Shared.Commands;

public class CreateSellerCommand : ICommand<CreateSellerCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
}
