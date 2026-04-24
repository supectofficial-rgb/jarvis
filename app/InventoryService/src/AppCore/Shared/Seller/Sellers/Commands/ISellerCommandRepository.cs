namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Commands;

using Insurance.InventoryService.AppCore.Domain.Seller.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ISellerCommandRepository : ICommandRepository<Seller, long>
{
    Task<Seller?> GetByBusinessKeyAsync(Guid sellerBusinessKey);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}
