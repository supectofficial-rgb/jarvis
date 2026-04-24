namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.ActivateQualityStatus;

using OysterFx.AppCore.Shared.Commands;

public class ActivateQualityStatusCommand : ICommand<ActivateQualityStatusCommandResult>
{
    public Guid QualityStatusBusinessKey { get; set; }
}
