namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventoryTransactionQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IInventoryTransactionQueryRepository
{
    public InventoryTransactionQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetInventoryTransactionByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid transactionBusinessKey)
    {
        var item = await _dbContext.Set<InventoryTransactionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == transactionBusinessKey);

        return item is null ? null : ToBusinessKeyResult(item);
    }

    public async Task<InventoryTransactionListItem?> GetByIdAsync(Guid transactionId)
    {
        var item = await _dbContext.Set<InventoryTransactionReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == transactionId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<InventoryTransactionListItem?> GetByNoAsync(string transactionNo)
    {
        if (string.IsNullOrWhiteSpace(transactionNo))
            return null;

        var normalized = transactionNo.Trim();
        var item = await _dbContext.Set<InventoryTransactionReadModel>()
            .FirstOrDefaultAsync(x => x.TransactionNo == normalized);

        return item is null ? null : ToListItem(item);
    }

    public async Task<SearchInventoryTransactionsQueryResult> SearchAsync(SearchInventoryTransactionsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<InventoryTransactionReadModel> dbQuery = _dbContext.Set<InventoryTransactionReadModel>();

        if (!string.IsNullOrWhiteSpace(query.TransactionNo))
        {
            var transactionNo = query.TransactionNo.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.TransactionNo, $"%{transactionNo}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.TransactionType) && Enum.TryParse<Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities.InventoryTransactionType>(query.TransactionType, true, out var transactionType))
            dbQuery = dbQuery.Where(x => x.TransactionType == transactionType);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities.InventoryTransactionStatus>(query.Status, true, out var status))
            dbQuery = dbQuery.Where(x => x.Status == status);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.OccurredFrom.HasValue)
            dbQuery = dbQuery.Where(x => x.OccurredAt >= query.OccurredFrom.Value);

        if (query.OccurredTo.HasValue)
            dbQuery = dbQuery.Where(x => x.OccurredAt <= query.OccurredTo.Value);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .OrderByDescending(x => x.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new SearchInventoryTransactionsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(ToListItem).ToList()
        };
    }

    public async Task<List<InventoryTransactionListItem>> GetByVariantAsync(Guid variantRef)
    {
        var txIds = await _dbContext.Set<InventoryTransactionLineReadModel>()
            .Where(x => x.VariantRef == variantRef)
            .Select(x => x.InventoryTransactionId)
            .Distinct()
            .ToListAsync();

        if (txIds.Count == 0)
            return new List<InventoryTransactionListItem>();

        var items = await _dbContext.Set<InventoryTransactionReadModel>()
            .Where(x => txIds.Contains(x.Id))
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<InventoryTransactionListItem>> GetByWarehouseAsync(Guid warehouseRef)
    {
        var items = await _dbContext.Set<InventoryTransactionReadModel>()
            .Where(x => x.WarehouseRef == warehouseRef)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    private static InventoryTransactionListItem ToListItem(InventoryTransactionReadModel x)
        => new()
        {
            TransactionBusinessKey = x.BusinessKey,
            TransactionNo = x.TransactionNo,
            TransactionType = x.TransactionType.ToString(),
            Status = x.Status.ToString(),
            ReferenceType = x.ReferenceType,
            ReferenceBusinessId = x.ReferenceBusinessId,
            WarehouseRef = x.WarehouseRef,
            SellerRef = x.SellerRef,
            OccurredAt = x.OccurredAt,
            PostedAt = x.PostedAt,
            ReasonCode = x.ReasonCode,
            ReversedTransactionRef = x.ReversedTransactionRef
        };

    private static GetInventoryTransactionByBusinessKeyQueryResult ToBusinessKeyResult(InventoryTransactionReadModel x)
        => new()
        {
            TransactionBusinessKey = x.BusinessKey,
            TransactionNo = x.TransactionNo,
            TransactionType = x.TransactionType.ToString(),
            Status = x.Status.ToString(),
            ReferenceType = x.ReferenceType,
            ReferenceBusinessId = x.ReferenceBusinessId,
            WarehouseRef = x.WarehouseRef,
            SellerRef = x.SellerRef,
            OccurredAt = x.OccurredAt,
            PostedAt = x.PostedAt,
            ReasonCode = x.ReasonCode,
            ReversedTransactionRef = x.ReversedTransactionRef
        };
}
