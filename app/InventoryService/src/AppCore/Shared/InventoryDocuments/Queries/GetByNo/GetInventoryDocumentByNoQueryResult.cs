namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByNo;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class GetInventoryDocumentByNoQueryResult
{
    public InventoryDocumentListItem Item { get; set; } = new();
}
