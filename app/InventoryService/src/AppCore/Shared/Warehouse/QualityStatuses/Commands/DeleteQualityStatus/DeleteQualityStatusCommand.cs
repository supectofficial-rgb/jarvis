namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeleteQualityStatus;

using OysterFx.AppCore.Shared.Commands;

public class DeleteQualityStatusCommand : ICommand<DeleteQualityStatusCommandResult>
{
    public Guid QualityStatusBusinessKey { get; set; }
}
