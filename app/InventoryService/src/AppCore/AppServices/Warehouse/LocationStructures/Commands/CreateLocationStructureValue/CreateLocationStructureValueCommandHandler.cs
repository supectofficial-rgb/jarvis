namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Commands.CreateLocationStructureValue;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureValue;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateLocationStructureValueCommandHandler : CommandHandler<CreateLocationStructureValueCommand, CreateLocationStructureValueCommandResult>
{
    private readonly ILocationStructureCommandRepository _repository;

    public CreateLocationStructureValueCommandHandler(ILocationStructureCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateLocationStructureValueCommandResult>> Handle(CreateLocationStructureValueCommand command)
    {
        if (command.StructureRef == Guid.Empty)
            return Fail("StructureRef is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (await _repository.GetNodeByBusinessKeyAsync(command.StructureRef) is null)
            return Fail("Structure node was not found.");

        if (await _repository.ExistsValueCodeAsync(command.StructureRef, command.Code))
            return Fail($"Structure value code '{command.Code.Trim()}' already exists.");

        try
        {
            var aggregate = LocationStructureValue.Create(
                command.StructureRef,
                command.Code,
                command.Name,
                command.DisplayOrder);

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreateLocationStructureValueCommandResult
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
            return Fail($"Creating structure value failed: {ex.Message}");
        }
    }
}
