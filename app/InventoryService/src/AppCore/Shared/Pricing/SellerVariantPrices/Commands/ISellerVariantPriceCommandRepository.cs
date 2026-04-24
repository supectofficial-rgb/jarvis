namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ISellerVariantPriceCommandRepository : ICommandRepository<SellerVariantPrice, long>
{
}
