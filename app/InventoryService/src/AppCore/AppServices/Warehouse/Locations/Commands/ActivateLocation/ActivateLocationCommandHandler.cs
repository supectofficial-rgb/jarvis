namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.ActivateLocation;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ActivateLocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateLocationCommandHandler : CommandHandler<ActivateLocationCommand, ActivateLocationCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public ActivateLocationCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateLocationCommandResult>> Handle(ActivateLocationCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        try
        {
            var aggregate = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
            if (aggregate is null)
                return Fail("Location was not found.");

            aggregate.Activate();
            await _repository.CommitAsync();

            return Ok(new ActivateLocationCommandResult
            {
                LocationBusinessKey = aggregate.BusinessKey.Value,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Activating location failed: {ex.Message}");
        }
    }
}
