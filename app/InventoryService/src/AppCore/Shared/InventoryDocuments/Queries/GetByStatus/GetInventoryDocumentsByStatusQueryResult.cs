namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class GetInventoryDocumentsByStatusQueryResult
{
    public List<InventoryDocumentListItem> Items { get; set; } = new();
}
