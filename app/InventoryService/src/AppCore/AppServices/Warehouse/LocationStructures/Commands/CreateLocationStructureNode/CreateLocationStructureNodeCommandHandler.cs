namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Commands.CreateLocationStructureNode;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureNode;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateLocationStructureNodeCommandHandler : CommandHandler<CreateLocationStructureNodeCommand, CreateLocationStructureNodeCommandResult>
{
    private readonly ILocationStructureCommandRepository _repository;

    public CreateLocationStructureNodeCommandHandler(ILocationStructureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateLocationStructureNodeCommandResult>> Handle(CreateLocationStructureNodeCommand command)
    {
        if (command.WarehouseRef == Guid.Empty)
            return Fail("WarehouseRef is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (await _repository.ExistsNodeCodeAsync(command.WarehouseRef, command.Code))
            return Fail($"Structure code '{command.Code.Trim()}' already exists.");

        if (command.ParentStructureRef.HasValue)
        {
            var parent = await _repository.GetNodeByBusinessKeyAsync(command.ParentStructureRef.Value);
            if (parent is null)
                return Fail("Parent structure was not found.");
        }

        try
        {
            var aggregate = LocationStructureNode.Create(
                command.WarehouseRef,
                command.Code,
                command.Name,
                command.DisplayOrder,
                command.ParentStructureRef);

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreateLocationStructureNodeCommandResult
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
            return Fail($"Creating location structure failed: {ex.Message}");
        }
    }
}
