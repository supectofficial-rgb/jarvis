namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.CreateLocation;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.CreateLocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateLocationCommandHandler : CommandHandler<CreateLocationCommand, CreateLocationCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public CreateLocationCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateLocationCommandResult>> Handle(CreateLocationCommand command)
    {
        if (command.WarehouseRef == Guid.Empty)
            return Fail("WarehouseRef is required.");

        if (string.IsNullOrWhiteSpace(command.LocationCode))
            return Fail("LocationCode is required.");

        if (!Enum.TryParse<LocationType>(command.LocationType, true, out var locationType))
            return Fail("LocationType is invalid.");

        try
        {
            var normalizedCode = command.LocationCode.Trim();
            if (await _repository.ExistsByCodeAsync(normalizedCode))
                return Fail($"Location code '{normalizedCode}' already exists.");

            var aggregate = Domain.Warehouse.Entities.Location.Create(
                command.WarehouseRef,
                normalizedCode,
                locationType,
                command.Aisle,
                command.Rack,
                command.Shelf,
                command.Bin);

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreateLocationCommandResult
            {
                LocationBusinessKey = aggregate.BusinessKey.Value,
                WarehouseRef = aggregate.WarehouseRef,
                LocationCode = aggregate.LocationCode,
                LocationType = aggregate.LocationType.ToString(),
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Creating location failed: {ex.Message}");
        }
    }
}
