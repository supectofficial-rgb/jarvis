namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Commands.UpdateLocationStructureValue;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureValue;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpdateLocationStructureValueCommandHandler : CommandHandler<UpdateLocationStructureValueCommand, UpdateLocationStructureValueCommandResult>
{
    private readonly ILocationStructureCommandRepository _repository;

    public UpdateLocationStructureValueCommandHandler(ILocationStructureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateLocationStructureValueCommandResult>> Handle(UpdateLocationStructureValueCommand command)
    {
        if (command.LocationStructureValueBusinessKey == Guid.Empty)
            return Fail("LocationStructureValueBusinessKey is required.");

        if (command.StructureRef == Guid.Empty)
            return Fail("StructureRef is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var aggregate = await _repository.GetValueByBusinessKeyAsync(command.LocationStructureValueBusinessKey);
            if (aggregate is null)
                return Fail("Structure value was not found.");

            if (!string.Equals(aggregate.Code, command.Code.Trim(), StringComparison.OrdinalIgnoreCase)
                && await _repository.ExistsValueCodeAsync(command.StructureRef, command.Code, command.LocationStructureValueBusinessKey))
            {
                return Fail($"Structure value code '{command.Code.Trim()}' already exists.");
            }

            aggregate.ChangeStructure(command.StructureRef);
            aggregate.ChangeCode(command.Code);
            aggregate.Rename(command.Name);
            aggregate.ChangeDisplayOrder(command.DisplayOrder);

            if (command.IsActive)
                aggregate.Activate();
            else
                aggregate.Deactivate();

            await _repository.CommitAsync();

            return Ok(new UpdateLocationStructureValueCommandResult
            {
                LocationStructureValueBusinessKey = aggregate.BusinessKey.Value,
                StructureRef = aggregate.StructureRef,
                Code = aggregate.Code,
                Name = aggregate.Name,
                DisplayOrder = aggregate.DisplayOrder,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Updating structure value failed: {ex.Message}");
        }
    }
}
