namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Commands.UpdateQualityStatus;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.UpdateQualityStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateQualityStatusCommandHandler : CommandHandler<UpdateQualityStatusCommand, UpdateQualityStatusCommandResult>
{
    private readonly IQualityStatusCommandRepository _repository;

    public UpdateQualityStatusCommandHandler(IQualityStatusCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateQualityStatusCommandResult>> Handle(UpdateQualityStatusCommand command)
    {
        if (command.QualityStatusBusinessKey == Guid.Empty)
            return Fail("QualityStatusBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.QualityStatusBusinessKey);
        if (aggregate is null)
            return Fail("Quality status was not found.");

        var normalizedCode = command.Code.Trim();
        if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(normalizedCode, command.QualityStatusBusinessKey))
        {
            return Fail($"Quality status code '{normalizedCode}' already exists.");
        }

        aggregate.ChangeCode(normalizedCode);
        aggregate.Rename(command.Name.Trim());

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        await _repository.CommitAsync();

        return Ok(new UpdateQualityStatusCommandResult
        {
            QualityStatusBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            IsActive = aggregate.IsActive
        });
    }
}
