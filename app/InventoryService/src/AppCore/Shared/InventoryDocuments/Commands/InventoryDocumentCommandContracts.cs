namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;

public class InventoryDocumentCommandLineItem
{
    public Guid VariantRef { get; set; }
    public decimal Qty { get; set; }
    public Guid UomRef { get; set; }
    public decimal BaseQty { get; set; }
    public Guid BaseUomRef { get; set; }
    public Guid? SourceLocationRef { get; set; }
    public Guid? DestinationLocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public Guid? FromQualityStatusRef { get; set; }
    public Guid? ToQualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ReasonCode { get; set; }
    public string? AdjustmentDirection { get; set; }
    public List<InventoryDocumentCommandLineSerialItem> Serials { get; set; } = new();
}

public class InventoryDocumentCommandLineSerialItem
{
    public Guid? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
}
