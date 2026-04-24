namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.CreatePriceChannel;

public class CreatePriceChannelCommandResult
{
    public Guid PriceChannelBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
