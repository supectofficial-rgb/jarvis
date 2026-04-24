namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeactivateQualityStatus;

using OysterFx.AppCore.Shared.Commands;

public class DeactivateQualityStatusCommand : ICommand<DeactivateQualityStatusCommandResult>
{
    public Guid QualityStatusBusinessKey { get; set; }
}
