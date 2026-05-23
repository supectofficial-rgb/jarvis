namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;

public class InventoryDocumentLineSerialReadModel
{
    public long Id { get; set; }
    public Guid DocumentLineRef { get; set; }
    public Guid? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public long? InventoryDocumentLineId { get; set; }
    public Guid BusinessKey { get; set; }
}
