namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;

public class FulfillmentCommandLineItem
{
    public Guid VariantRef { get; set; }
    public decimal Qty { get; set; }
    public Guid UomRef { get; set; }
    public decimal BaseQty { get; set; }
    public Guid BaseUomRef { get; set; }
    public Guid? SourceLocationRef { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? ReservationAllocationRef { get; set; }
    public List<FulfillmentCommandLineSerialItem> Serials { get; set; } = new();
}

public class FulfillmentCommandLineSerialItem
{
    public Guid? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
}
