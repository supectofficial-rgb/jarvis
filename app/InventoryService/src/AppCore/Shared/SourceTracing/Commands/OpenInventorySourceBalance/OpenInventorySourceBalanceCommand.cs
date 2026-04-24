namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands.OpenInventorySourceBalance;

using OysterFx.AppCore.Shared.Commands;

public class OpenInventorySourceBalanceCommand : ICommand<Guid>
{
    public string SourceType { get; set; } = string.Empty;
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public Guid BaseUomRef { get; set; }
    public decimal ReceivedQty { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? SourceDocumentRef { get; set; }
    public Guid? SourceDocumentLineRef { get; set; }
    public Guid? SourceTransactionRef { get; set; }
    public Guid? SourceTransactionLineRef { get; set; }
    public Guid? SerialRef { get; set; }
}
