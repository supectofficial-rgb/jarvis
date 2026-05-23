namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Commands.UpdateLocationStructureNode;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureNode;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpdateLocationStructureNodeCommandHandler : CommandHandler<UpdateLocationStructureNodeCommand, UpdateLocationStructureNodeCommandResult>
{
    private readonly ILocationStructureCommandRepository _repository;

    public UpdateLocationStructureNodeCommandHandler(ILocationStructureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateLocationStructureNodeCommandResult>> Handle(UpdateLocationStructureNodeCommand command)
    {
        if (command.LocationStructureBusinessKey == Guid.Empty)
            return Fail("LocationStructureBusinessKey is required.");

        if (command.WarehouseRef == Guid.Empty)
            return Fail("WarehouseRef is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var aggregate = await _repository.GetNodeByBusinessKeyAsync(command.LocationStructureBusinessKey);
            if (aggregate is null)
                return Fail("Structure node was not found.");

            if (!string.Equals(aggregate.Code, command.Code.Trim(), StringComparison.OrdinalIgnoreCase)
                && await _repository.ExistsNodeCodeAsync(command.WarehouseRef, command.Code, command.LocationStructureBusinessKey))
            {
                return Fail($"Structure code '{command.Code.Trim()}' already exists.");
            }

            if (command.ParentStructureRef.HasValue && command.ParentStructureRef.Value == command.LocationStructureBusinessKey)
                return Fail("Structure cannot be parent of itself.");

            aggregate.ChangeWarehouse(command.WarehouseRef);
            aggregate.ChangeParent(command.ParentStructureRef);
            aggregate.ChangeCode(command.Code);
            aggregate.Rename(command.Name);
            aggregate.ChangeDisplayOrder(command.DisplayOrder);

            if (command.IsActive)
                aggregate.Activate();
            else
                aggregate.Deactivate();

            await _repository.CommitAsync();

            return Ok(new UpdateLocationStructureNodeCommandResult
            {
                LocationStructureBusinessKey = aggregate.BusinessKey.Value,
                WarehouseRef = aggregate.WarehouseRef,
                ParentStructureRef = aggregate.ParentStructureRef,
                Code = aggregate.Code,
                Name = aggregate.Name,
                DisplayOrder = aggregate.DisplayOrder,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Updating location structure failed: {ex.Message}");
        }
    }
}
