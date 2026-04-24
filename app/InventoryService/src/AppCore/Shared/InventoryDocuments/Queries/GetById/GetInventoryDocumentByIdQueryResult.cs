namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class GetInventoryDocumentByIdQueryResult
{
    public InventoryDocumentListItem Item { get; set; } = new();
}
