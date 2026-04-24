namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.UpdateLocation;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.UpdateLocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateLocationCommandHandler : CommandHandler<UpdateLocationCommand, UpdateLocationCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public UpdateLocationCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateLocationCommandResult>> Handle(UpdateLocationCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        if (command.WarehouseRef == Guid.Empty)
            return Fail("WarehouseRef is required.");

        if (string.IsNullOrWhiteSpace(command.LocationCode))
            return Fail("LocationCode is required.");

        if (!Enum.TryParse<LocationType>(command.LocationType, true, out var locationType))
            return Fail("LocationType is invalid.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
        if (aggregate is null)
            return Fail("Location was not found.");

        var normalizedCode = command.LocationCode.Trim();
        if (!string.Equals(aggregate.LocationCode, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(normalizedCode, command.LocationBusinessKey))
        {
            return Fail($"Location code '{normalizedCode}' already exists.");
        }

        aggregate.ChangeWarehouse(command.WarehouseRef);
        aggregate.ChangeCode(normalizedCode);
        aggregate.ChangeType(locationType);
        aggregate.UpdateCoordinates(command.Aisle, command.Rack, command.Shelf, command.Bin);

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        await _repository.CommitAsync();

        return Ok(new UpdateLocationCommandResult
        {
            LocationBusinessKey = aggregate.BusinessKey.Value,
            LocationCode = aggregate.LocationCode,
            LocationType = aggregate.LocationType.ToString(),
            IsActive = aggregate.IsActive
        });
    }
}
