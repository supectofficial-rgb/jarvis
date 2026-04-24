namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Commands.ActivateQualityStatus;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.ActivateQualityStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateQualityStatusCommandHandler : CommandHandler<ActivateQualityStatusCommand, ActivateQualityStatusCommandResult>
{
    private readonly IQualityStatusCommandRepository _repository;

    public ActivateQualityStatusCommandHandler(IQualityStatusCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateQualityStatusCommandResult>> Handle(ActivateQualityStatusCommand command)
    {
        if (command.QualityStatusBusinessKey == Guid.Empty)
            return Fail("QualityStatusBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.QualityStatusBusinessKey);
        if (aggregate is null)
            return Fail("Quality status was not found.");

        aggregate.Activate();
        await _repository.CommitAsync();

        return Ok(new ActivateQualityStatusCommandResult
        {
            QualityStatusBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
