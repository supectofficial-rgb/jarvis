namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.CreatePriceType;

using OysterFx.AppCore.Shared.Commands;

public class CreatePriceTypeCommand : ICommand<CreatePriceTypeCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
