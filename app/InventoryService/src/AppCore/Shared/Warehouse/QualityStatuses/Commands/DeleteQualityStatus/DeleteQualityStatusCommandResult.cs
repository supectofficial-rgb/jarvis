namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeleteQualityStatus;

public class DeleteQualityStatusCommandResult
{
    public Guid QualityStatusBusinessKey { get; set; }
    public bool Deleted { get; set; }
}
