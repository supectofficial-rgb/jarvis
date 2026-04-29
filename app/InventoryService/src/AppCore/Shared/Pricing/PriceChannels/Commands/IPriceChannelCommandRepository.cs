namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IPriceChannelCommandRepository : ICommandRepository<PriceChannel, long>
{
    Task<PriceChannel?> GetByBusinessKeyAsync(Guid priceChannelBusinessKey);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}
