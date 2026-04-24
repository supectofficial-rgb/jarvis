namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeactivateQualityStatus;

public class DeactivateQualityStatusCommandResult
{
    public Guid QualityStatusBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
