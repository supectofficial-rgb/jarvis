namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands;

public class ReturnRequestCommandLineItem
{
    public Guid VariantRef { get; set; }
    public decimal Qty { get; set; }
    public Guid UomRef { get; set; }
    public decimal BaseQty { get; set; }
    public Guid BaseUomRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ExpectedCondition { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public List<ReturnRequestCommandLineSerialItem> Serials { get; set; } = new();
}

public class ReturnRequestCommandLineSerialItem
{
    public Guid? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
}
