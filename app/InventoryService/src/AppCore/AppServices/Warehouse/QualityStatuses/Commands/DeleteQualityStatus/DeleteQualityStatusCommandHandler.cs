namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Commands.DeleteQualityStatus;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeleteQualityStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteQualityStatusCommandHandler : CommandHandler<DeleteQualityStatusCommand, DeleteQualityStatusCommandResult>
{
    private readonly IQualityStatusCommandRepository _repository;

    public DeleteQualityStatusCommandHandler(IQualityStatusCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeleteQualityStatusCommandResult>> Handle(DeleteQualityStatusCommand command)
    {
        if (command.QualityStatusBusinessKey == Guid.Empty)
            return Fail("QualityStatusBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.QualityStatusBusinessKey);
        if (aggregate is null)
            return Fail("Quality status was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeleteQualityStatusCommandResult
        {
            QualityStatusBusinessKey = aggregate.BusinessKey.Value,
            Deleted = true
        });
    }
}
