namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.MoveLocationToWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.MoveLocationToWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class MoveLocationToWarehouseCommandHandler : CommandHandler<MoveLocationToWarehouseCommand, MoveLocationToWarehouseCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public MoveLocationToWarehouseCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<MoveLocationToWarehouseCommandResult>> Handle(MoveLocationToWarehouseCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        if (command.TargetWarehouseRef == Guid.Empty)
            return Fail("TargetWarehouseRef is required.");

        var location = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
        if (location is null)
            return Fail("Location was not found.");

        location.ChangeWarehouse(command.TargetWarehouseRef);
        await _repository.CommitAsync();

        return Ok(new MoveLocationToWarehouseCommandResult
        {
            LocationBusinessKey = location.BusinessKey.Value,
            WarehouseRef = location.WarehouseRef
        });
    }
}
