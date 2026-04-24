namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.UpdatePriceChannel;

using OysterFx.AppCore.Shared.Commands;

public class UpdatePriceChannelCommand : ICommand<UpdatePriceChannelCommandResult>
{
    public Guid PriceChannelBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
