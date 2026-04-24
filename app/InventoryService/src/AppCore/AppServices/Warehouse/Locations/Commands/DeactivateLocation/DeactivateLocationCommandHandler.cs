namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.DeactivateLocation;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeactivateLocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateLocationCommandHandler : CommandHandler<DeactivateLocationCommand, DeactivateLocationCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public DeactivateLocationCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeactivateLocationCommandResult>> Handle(DeactivateLocationCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
        if (aggregate is null)
            return Fail("Location was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateLocationCommandResult
        {
            LocationBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
