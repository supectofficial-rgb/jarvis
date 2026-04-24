namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Seller.Sellers;

using Insurance.InventoryService.AppCore.Domain.Seller.Entities;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class SellerCommandRepository : CommandRepository<Seller, InventoryServiceCommandDbContext>, ISellerCommandRepository
{
    public SellerCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Seller?> GetByBusinessKeyAsync(Guid sellerBusinessKey)
    {
        return _dbContext.Set<Seller>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == sellerBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<Seller>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}
