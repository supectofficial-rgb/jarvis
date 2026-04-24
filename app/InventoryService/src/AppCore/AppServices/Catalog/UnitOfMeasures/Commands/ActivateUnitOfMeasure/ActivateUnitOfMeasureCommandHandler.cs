namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Commands.ActivateUnitOfMeasure;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.ActivateUnitOfMeasure;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateUnitOfMeasureCommandHandler : CommandHandler<ActivateUnitOfMeasureCommand, ActivateUnitOfMeasureCommandResult>
{
    private readonly IUnitOfMeasureCommandRepository _repository;

    public ActivateUnitOfMeasureCommandHandler(IUnitOfMeasureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateUnitOfMeasureCommandResult>> Handle(ActivateUnitOfMeasureCommand command)
    {
        if (command.UnitOfMeasureBusinessKey == Guid.Empty)
            return Fail("UnitOfMeasureBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.UnitOfMeasureBusinessKey);
        if (aggregate is null)
            return Fail("Unit of measure was not found.");

        aggregate.Activate();
        await _repository.CommitAsync();

        return Ok(new ActivateUnitOfMeasureCommandResult
        {
            UnitOfMeasureBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
