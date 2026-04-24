namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.ChangeLocationType;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ChangeLocationType;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ChangeLocationTypeCommandHandler : CommandHandler<ChangeLocationTypeCommand, ChangeLocationTypeCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public ChangeLocationTypeCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ChangeLocationTypeCommandResult>> Handle(ChangeLocationTypeCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        if (!Enum.TryParse<LocationType>(command.LocationType, true, out var locationType))
            return Fail("LocationType is invalid.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
        if (aggregate is null)
            return Fail("Location was not found.");

        aggregate.ChangeType(locationType);
        await _repository.CommitAsync();

        return Ok(new ChangeLocationTypeCommandResult
        {
            LocationBusinessKey = aggregate.BusinessKey.Value,
            LocationType = aggregate.LocationType.ToString()
        });
    }
}
