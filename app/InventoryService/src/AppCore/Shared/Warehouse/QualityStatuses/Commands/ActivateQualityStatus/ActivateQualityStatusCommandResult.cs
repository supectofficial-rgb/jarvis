namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.ActivateQualityStatus;

public class ActivateQualityStatusCommandResult
{
    public Guid QualityStatusBusinessKey { get; set; }
    public bool IsActive { get; set; }
}
