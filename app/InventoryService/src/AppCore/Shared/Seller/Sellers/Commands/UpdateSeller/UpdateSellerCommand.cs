namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands.UpdateSeller;

using OysterFx.AppCore.Shared.Commands;

public class UpdateSellerCommand : ICommand<UpdateSellerCommandResult>
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; } = true;
}
