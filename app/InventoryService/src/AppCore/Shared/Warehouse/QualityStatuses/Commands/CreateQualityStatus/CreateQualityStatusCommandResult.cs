namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.CreateQualityStatus;

public class CreateQualityStatusCommandResult
{
    public Guid QualityStatusBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
