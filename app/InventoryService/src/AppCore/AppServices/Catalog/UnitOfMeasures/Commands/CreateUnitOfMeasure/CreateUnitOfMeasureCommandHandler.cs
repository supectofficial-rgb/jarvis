namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Commands.CreateUnitOfMeasure;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.CreateUnitOfMeasure;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateUnitOfMeasureCommandHandler : CommandHandler<CreateUnitOfMeasureCommand, CreateUnitOfMeasureCommandResult>
{
    private readonly IUnitOfMeasureCommandRepository _repository;

    public CreateUnitOfMeasureCommandHandler(IUnitOfMeasureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateUnitOfMeasureCommandResult>> Handle(CreateUnitOfMeasureCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (command.Precision < 0)
            return Fail("Precision must be greater than or equal to zero.");

        var normalizedCode = command.Code.Trim();
        if (await _repository.ExistsByCodeAsync(normalizedCode))
            return Fail($"Unit of measure code '{normalizedCode}' already exists.");

        var aggregate = Domain.Catalog.Entities.UnitOfMeasure.Create(normalizedCode, command.Name.Trim(), command.Precision);

        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new CreateUnitOfMeasureCommandResult
        {
            UnitOfMeasureBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            Precision = aggregate.Precision,
            IsActive = aggregate.IsActive
        });
    }
}
