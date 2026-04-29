namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IPriceTypeCommandRepository : ICommandRepository<PriceType, long>
{
    Task<PriceType?> GetByBusinessKeyAsync(Guid priceTypeBusinessKey);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}
