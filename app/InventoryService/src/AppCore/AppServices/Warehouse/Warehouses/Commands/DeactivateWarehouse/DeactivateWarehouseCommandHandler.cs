namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Commands.DeactivateWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeactivateWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateWarehouseCommandHandler : CommandHandler<DeactivateWarehouseCommand, DeactivateWarehouseCommandResult>
{
    private readonly IWarehouseCommandRepository _repository;

    public DeactivateWarehouseCommandHandler(IWarehouseCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeactivateWarehouseCommandResult>> Handle(DeactivateWarehouseCommand command)
    {
        if (command.WarehouseBusinessKey == Guid.Empty)
            return Fail("WarehouseBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.WarehouseBusinessKey);
        if (aggregate is null)
            return Fail("Warehouse was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateWarehouseCommandResult
        {
            WarehouseBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
