namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.CreateQualityStatus;

using OysterFx.AppCore.Shared.Commands;

public class CreateQualityStatusCommand : ICommand<CreateQualityStatusCommandResult>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
