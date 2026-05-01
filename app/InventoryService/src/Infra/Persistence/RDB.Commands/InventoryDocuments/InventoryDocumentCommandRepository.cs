namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InventoryDocumentCommandRepository : CommandRepository<InventoryDocument, InventoryServiceCommandDbContext>, IInventoryDocumentCommandRepository
{
    public InventoryDocumentCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<InventoryDocument?> GetByBusinessKeyAsync(Guid documentBusinessKey)
    {
        return _dbContext.InventoryDocuments
            .Include(x => x.Lines)
            .ThenInclude(x => x.Serials)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(documentBusinessKey));
    }

    public Task<bool> ExistsByDocumentNoAsync(string documentNo, Guid? exceptBusinessKey = null)
    {
        if (string.IsNullOrWhiteSpace(documentNo))
            return Task.FromResult(false);

        var normalized = documentNo.Trim();
        var query = _dbContext.InventoryDocuments.Where(x => x.DocumentNo == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}
