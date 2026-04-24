namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.CreatePriceChannel;

using OysterFx.AppCore.Shared.Commands;

public class CreatePriceChannelCommand : ICommand<CreatePriceChannelCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
