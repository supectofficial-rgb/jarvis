namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.UpdatePriceType;

using OysterFx.AppCore.Shared.Commands;

public class UpdatePriceTypeCommand : ICommand<UpdatePriceTypeCommandResult>
{
    public Guid PriceTypeBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
