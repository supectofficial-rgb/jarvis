namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Entities;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;

public class ReturnRequestReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public ReturnRequestStatus Status { get; set; }
    public string? ReasonCode { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}
