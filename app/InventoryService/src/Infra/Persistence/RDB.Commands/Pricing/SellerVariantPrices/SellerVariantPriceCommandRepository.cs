namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.SellerVariantPrices;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class SellerVariantPriceCommandRepository : CommandRepository<SellerVariantPrice, InventoryServiceCommandDbContext>, ISellerVariantPriceCommandRepository
{
    public SellerVariantPriceCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<SellerVariantPrice?> GetByBusinessKeyAsync(Guid sellerVariantPriceBusinessKey)
    {
        return _dbContext.Set<SellerVariantPrice>()
            .Include(x => x.Offers)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(sellerVariantPriceBusinessKey));
    }

    public Task<bool> ExistsByPricingKeyAsync(
        Guid sellerRef,
        Guid variantRef,
        Guid priceTypeRef,
        Guid priceChannelRef,
        Guid? exceptBusinessKey = null)
    {
        var query = _dbContext.Set<SellerVariantPrice>()
            .Where(x =>
                x.SellerRef == sellerRef &&
                x.VariantRef == variantRef &&
                x.PriceTypeRef == priceTypeRef &&
                x.PriceChannelRef == priceChannelRef);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
