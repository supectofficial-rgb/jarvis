namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Commands.ActivateWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.ActivateWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateWarehouseCommandHandler : CommandHandler<ActivateWarehouseCommand, ActivateWarehouseCommandResult>
{
    private readonly IWarehouseCommandRepository _repository;

    public ActivateWarehouseCommandHandler(IWarehouseCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateWarehouseCommandResult>> Handle(ActivateWarehouseCommand command)
    {
        if (command.WarehouseBusinessKey == Guid.Empty)
            return Fail("WarehouseBusinessKey is required.");

        try
        {
            var aggregate = await _repository.GetByBusinessKeyAsync(command.WarehouseBusinessKey);
            if (aggregate is null)
                return Fail("Warehouse was not found.");

            aggregate.Activate();
            await _repository.CommitAsync();

            return Ok(new ActivateWarehouseCommandResult
            {
                WarehouseBusinessKey = aggregate.BusinessKey.Value,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Activating warehouse failed: {ex.Message}");
        }
    }
}
