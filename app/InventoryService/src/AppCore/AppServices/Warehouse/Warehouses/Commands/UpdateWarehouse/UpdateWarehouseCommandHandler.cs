namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Commands.UpdateWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.UpdateWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateWarehouseCommandHandler : CommandHandler<UpdateWarehouseCommand, UpdateWarehouseCommandResult>
{
    private readonly IWarehouseCommandRepository _repository;

    public UpdateWarehouseCommandHandler(IWarehouseCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateWarehouseCommandResult>> Handle(UpdateWarehouseCommand command)
    {
        if (command.WarehouseBusinessKey == Guid.Empty)
            return Fail("WarehouseBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var aggregate = await _repository.GetByBusinessKeyAsync(command.WarehouseBusinessKey);
            if (aggregate is null)
                return Fail("Warehouse was not found.");

            var normalizedCode = command.Code.Trim();
            if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
                && await _repository.ExistsByCodeAsync(normalizedCode, command.WarehouseBusinessKey))
            {
                return Fail($"Warehouse code '{normalizedCode}' already exists.");
            }

            aggregate.ChangeCode(normalizedCode);
            aggregate.Rename(command.Name.Trim());

            if (command.IsActive)
                aggregate.Activate();
            else
                aggregate.Deactivate();

            await _repository.CommitAsync();

            return Ok(new UpdateWarehouseCommandResult
            {
                WarehouseBusinessKey = aggregate.BusinessKey.Value,
                Code = aggregate.Code,
                Name = aggregate.Name,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Updating warehouse failed: {ex.Message}");
        }
    }
}
