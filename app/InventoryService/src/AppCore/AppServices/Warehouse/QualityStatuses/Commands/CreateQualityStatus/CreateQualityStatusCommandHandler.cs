namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.QualityStatuses.Commands.CreateQualityStatus;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.CreateQualityStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateQualityStatusCommandHandler : CommandHandler<CreateQualityStatusCommand, CreateQualityStatusCommandResult>
{
    private readonly IQualityStatusCommandRepository _repository;

    public CreateQualityStatusCommandHandler(IQualityStatusCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateQualityStatusCommandResult>> Handle(CreateQualityStatusCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var normalizedCode = command.Code.Trim();
        if (await _repository.ExistsByCodeAsync(normalizedCode))
            return Fail($"Quality status code '{normalizedCode}' already exists.");

        var aggregate = Domain.Warehouse.Entities.QualityStatus.Create(normalizedCode, command.Name.Trim());

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new CreateQualityStatusCommandResult
        {
            QualityStatusBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            IsActive = aggregate.IsActive
        });
    }
}
