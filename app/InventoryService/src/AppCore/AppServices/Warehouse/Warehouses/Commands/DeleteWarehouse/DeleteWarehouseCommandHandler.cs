namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Commands.DeleteWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeleteWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteWarehouseCommandHandler : CommandHandler<DeleteWarehouseCommand, DeleteWarehouseCommandResult>
{
    private readonly IWarehouseCommandRepository _repository;

    public DeleteWarehouseCommandHandler(IWarehouseCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeleteWarehouseCommandResult>> Handle(DeleteWarehouseCommand command)
    {
        if (command.WarehouseBusinessKey == Guid.Empty)
            return Fail("WarehouseBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.WarehouseBusinessKey);
        if (aggregate is null)
            return Fail("Warehouse was not found.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeleteWarehouseCommandResult
        {
            WarehouseBusinessKey = aggregate.BusinessKey.Value,
            Deleted = true
        });
    }
}
