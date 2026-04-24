namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.CreateSerialItem;

using OysterFx.AppCore.Shared.Commands;

public class CreateSerialItemCommand : ICommand<CreateSerialItemCommandResult>
{
    public string SerialNo { get; set; } = string.Empty;
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
}
