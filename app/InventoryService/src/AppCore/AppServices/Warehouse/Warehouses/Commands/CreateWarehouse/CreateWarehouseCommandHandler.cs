namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Commands.CreateWarehouse;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.CreateWarehouse;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateWarehouseCommandHandler : CommandHandler<CreateWarehouseCommand, CreateWarehouseCommandResult>
{
    private readonly IWarehouseCommandRepository _repository;

    public CreateWarehouseCommandHandler(IWarehouseCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateWarehouseCommandResult>> Handle(CreateWarehouseCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var normalizedCode = command.Code.Trim();
            if (await _repository.ExistsByCodeAsync(normalizedCode))
                return Fail($"Warehouse code '{normalizedCode}' already exists.");

            var aggregate = Domain.Warehouse.Entities.Warehouse.Create(normalizedCode, command.Name.Trim());

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreateWarehouseCommandResult
            {
                WarehouseBusinessKey = aggregate.BusinessKey.Value,
                Code = aggregate.Code,
                Name = aggregate.Name,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Creating warehouse failed: {ex.Message}");
        }
    }
}
