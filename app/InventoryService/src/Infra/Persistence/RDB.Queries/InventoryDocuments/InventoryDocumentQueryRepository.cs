namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetLinesByDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventoryDocumentQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IInventoryDocumentQueryRepository
{
    public InventoryDocumentQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetInventoryDocumentByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid businessKey)
    {
        var item = await _dbContext.InventoryDocuments
            .FirstOrDefaultAsync(x => x.BusinessKey == businessKey);

        return item is null ? null : ToBusinessKeyResult(item);
    }

    public async Task<InventoryDocumentListItem?> GetByIdAsync(Guid documentId)
    {
        var item = await _dbContext.InventoryDocuments
            .FirstOrDefaultAsync(x => x.BusinessKey == documentId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<InventoryDocumentListItem?> GetByNoAsync(string documentNo)
    {
        if (string.IsNullOrWhiteSpace(documentNo))
            return null;

        var normalized = documentNo.Trim();
        var item = await _dbContext.InventoryDocuments
            .FirstOrDefaultAsync(x => x.DocumentNo == normalized);

        return item is null ? null : ToListItem(item);
    }

    public async Task<GetInventoryDocumentLinesByDocumentQueryResult> GetLinesByDocumentAsync(Guid documentBusinessKey)
    {
        var document = await _dbContext.InventoryDocuments
            .FirstOrDefaultAsync(x => x.BusinessKey == documentBusinessKey);

        if (document is null)
            return new GetInventoryDocumentLinesByDocumentQueryResult();

        var lines = await _dbContext.InventoryDocumentLines
            .Where(x => x.InventoryDocumentId == document.Id)
            .OrderBy(x => x.Id)
            .Select(x => new InventoryDocumentLineListItem
            {
                LineBusinessKey = x.BusinessKey,
                VariantRef = x.VariantRef,
                Qty = x.Qty,
                UomRef = x.UomRef,
                BaseQty = x.BaseQty,
                BaseUomRef = x.BaseUomRef,
                SourceLocationRef = x.SourceLocationRef,
                DestinationLocationRef = x.DestinationLocationRef,
                QualityStatusRef = x.QualityStatusRef,
                FromQualityStatusRef = x.FromQualityStatusRef,
                ToQualityStatusRef = x.ToQualityStatusRef,
                LotBatchNo = x.LotBatchNo,
                ReasonCode = x.ReasonCode,
                AdjustmentDirection = x.AdjustmentDirection?.ToString()
            })
            .ToListAsync();

        return new GetInventoryDocumentLinesByDocumentQueryResult { Items = lines };
    }

    public async Task<SearchInventoryDocumentsQueryResult> SearchAsync(SearchInventoryDocumentsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<InventoryDocumentReadModel> dbQuery = _dbContext.InventoryDocuments;

        if (!string.IsNullOrWhiteSpace(query.DocumentNo))
        {
            var documentNo = query.DocumentNo.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.DocumentNo, $"%{documentNo}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.DocumentType) && Enum.TryParse<InventoryDocumentType>(query.DocumentType, true, out var documentType))
            dbQuery = dbQuery.Where(x => x.DocumentType == documentType);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<InventoryDocumentStatus>(query.Status, true, out var status))
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

        return new SearchInventoryDocumentsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(ToListItem).ToList()
        };
    }

    public async Task<List<InventoryDocumentListItem>> GetByTypeAsync(string documentType)
    {
        if (!Enum.TryParse<InventoryDocumentType>(documentType, true, out var type))
            return new List<InventoryDocumentListItem>();

        var items = await _dbContext.InventoryDocuments
            .Where(x => x.DocumentType == type)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<InventoryDocumentListItem>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<InventoryDocumentStatus>(status, true, out var parsedStatus))
            return new List<InventoryDocumentListItem>();

        var items = await _dbContext.InventoryDocuments
            .Where(x => x.Status == parsedStatus)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    private static InventoryDocumentListItem ToListItem(InventoryDocumentReadModel x)
        => new()
        {
            DocumentBusinessKey = x.BusinessKey,
            DocumentNo = x.DocumentNo,
            DocumentType = x.DocumentType.ToString(),
            Status = x.Status.ToString(),
            ReferenceType = x.ReferenceType,
            ReferenceBusinessId = x.ReferenceBusinessId,
            WarehouseRef = x.WarehouseRef,
            SellerRef = x.SellerRef,
            OccurredAt = x.OccurredAt,
            ApprovedAt = x.ApprovedAt,
            PostedAt = x.PostedAt,
            PostedTransactionRef = x.PostedTransactionRef,
            ReasonCode = x.ReasonCode
        };

    private static GetInventoryDocumentByBusinessKeyQueryResult ToBusinessKeyResult(InventoryDocumentReadModel x)
        => new()
        {
            DocumentBusinessKey = x.BusinessKey,
            DocumentNo = x.DocumentNo,
            DocumentType = x.DocumentType.ToString(),
            Status = x.Status.ToString(),
            ReferenceType = x.ReferenceType,
            ReferenceBusinessId = x.ReferenceBusinessId,
            WarehouseRef = x.WarehouseRef,
            SellerRef = x.SellerRef,
            OccurredAt = x.OccurredAt,
            ApprovedAt = x.ApprovedAt,
            PostedAt = x.PostedAt,
            PostedTransactionRef = x.PostedTransactionRef,
            ReasonCode = x.ReasonCode
        };
}
