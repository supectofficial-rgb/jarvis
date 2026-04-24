namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByType;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class GetInventoryDocumentsByTypeQueryResult
{
    public List<InventoryDocumentListItem> Items { get; set; } = new();
}
