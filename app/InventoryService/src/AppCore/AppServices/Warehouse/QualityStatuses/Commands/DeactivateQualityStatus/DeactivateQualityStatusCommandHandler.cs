namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Commands.DeactivateQualityStatus;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeactivateQualityStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateQualityStatusCommandHandler : CommandHandler<DeactivateQualityStatusCommand, DeactivateQualityStatusCommandResult>
{
    private readonly IQualityStatusCommandRepository _repository;

    public DeactivateQualityStatusCommandHandler(IQualityStatusCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeactivateQualityStatusCommandResult>> Handle(DeactivateQualityStatusCommand command)
    {
        if (command.QualityStatusBusinessKey == Guid.Empty)
            return Fail("QualityStatusBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.QualityStatusBusinessKey);
        if (aggregate is null)
            return Fail("Quality status was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateQualityStatusCommandResult
        {
            QualityStatusBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
