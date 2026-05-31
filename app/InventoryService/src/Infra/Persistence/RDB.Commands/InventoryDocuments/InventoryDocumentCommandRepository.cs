namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments;

using System.Data.Common;
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

    public async Task<string> GetNextDocumentNoAsync()
    {
        const string sequenceName = "\"InventoryDocumentNoSequence\"";
        var connection = _dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

        try
        {
            if (shouldCloseConnection)
            {
                await connection.OpenAsync();
            }

            await using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT nextval('{sequenceName}')";

            var result = await command.ExecuteScalarAsync();
            var nextValue = Convert.ToInt64(result ?? 0L);
            return $"INV-{nextValue:000000}";
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    public Task<bool> ExistsLineByVariantRefAsync(Guid variantRef)
    {
        if (variantRef == Guid.Empty)
            return Task.FromResult(false);

        return _dbContext.Set<InventoryDocumentLine>().AnyAsync(x => x.VariantRef == variantRef);
    }

    public async Task<bool> DeleteByBusinessKeyAsync(Guid documentBusinessKey)
    {
        if (documentBusinessKey == Guid.Empty)
        {
            return false;
        }

        var businessKey = BusinessKey.FromGuid(documentBusinessKey);
        var deleted = await _dbContext.Set<InventoryDocument>()
            .Where(x => x.BusinessKey == businessKey)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }
}
