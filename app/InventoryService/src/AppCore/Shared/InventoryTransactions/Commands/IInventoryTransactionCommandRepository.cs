namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventoryTransactionCommandRepository : ICommandRepository<InventoryTransaction, long>
{
    Task<InventoryTransaction?> GetByBusinessKeyAsync(Guid transactionBusinessKey);
    Task<bool> ExistsByTransactionNoAsync(string transactionNo, Guid? exceptBusinessKey = null);
}
