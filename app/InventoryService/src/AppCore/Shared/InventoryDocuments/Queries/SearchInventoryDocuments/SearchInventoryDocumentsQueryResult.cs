namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class SearchInventoryDocumentsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<InventoryDocumentListItem> Items { get; set; } = new();
}
