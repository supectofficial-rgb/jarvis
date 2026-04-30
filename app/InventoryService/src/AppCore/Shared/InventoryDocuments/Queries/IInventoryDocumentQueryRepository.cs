namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetLinesByDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;
using OysterFx.AppCore.Shared.Queries;

public interface IInventoryDocumentQueryRepository : IQueryRepository
{
    Task<GetInventoryDocumentByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid businessKey);
    Task<InventoryDocumentListItem?> GetByIdAsync(Guid documentId);
    Task<InventoryDocumentListItem?> GetByNoAsync(string documentNo);
    Task<GetInventoryDocumentLinesByDocumentQueryResult> GetLinesByDocumentAsync(Guid documentBusinessKey);
    Task<SearchInventoryDocumentsQueryResult> SearchAsync(SearchInventoryDocumentsQuery query);
    Task<List<InventoryDocumentListItem>> GetByTypeAsync(string documentType);
    Task<List<InventoryDocumentListItem>> GetByStatusAsync(string status);
}
