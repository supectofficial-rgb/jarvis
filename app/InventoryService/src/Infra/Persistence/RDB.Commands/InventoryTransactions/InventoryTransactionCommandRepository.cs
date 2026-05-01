namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryTransactions;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InventoryTransactionCommandRepository : CommandRepository<InventoryTransaction, InventoryServiceCommandDbContext>, IInventoryTransactionCommandRepository
{
    public InventoryTransactionCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<InventoryTransaction?> GetByBusinessKeyAsync(Guid transactionBusinessKey)
    {
        return _dbContext.InventoryTransactions
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(transactionBusinessKey));
    }

    public Task<bool> ExistsByTransactionNoAsync(string transactionNo, Guid? exceptBusinessKey = null)
    {
        if (string.IsNullOrWhiteSpace(transactionNo))
            return Task.FromResult(false);

        var normalized = transactionNo.Trim();
        var query = _dbContext.InventoryTransactions.Where(x => x.TransactionNo == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
