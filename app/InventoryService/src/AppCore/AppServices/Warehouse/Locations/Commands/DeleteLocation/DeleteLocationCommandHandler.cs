namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Commands.DeleteLocation;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeleteLocation;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteLocationCommandHandler : CommandHandler<DeleteLocationCommand, DeleteLocationCommandResult>
{
    private readonly ILocationCommandRepository _repository;

    public DeleteLocationCommandHandler(ILocationCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeleteLocationCommandResult>> Handle(DeleteLocationCommand command)
    {
        if (command.LocationBusinessKey == Guid.Empty)
            return Fail("LocationBusinessKey is required.");

        try
        {
            var aggregate = await _repository.GetByBusinessKeyAsync(command.LocationBusinessKey);
            if (aggregate is null)
                return Fail("Location was not found.");

            aggregate.Deactivate();
            await _repository.CommitAsync();

            return Ok(new DeleteLocationCommandResult
            {
                LocationBusinessKey = aggregate.BusinessKey.Value,
                Deleted = true
            });
        }
        catch (Exception ex)
        {
            return Fail($"Deleting location failed: {ex.Message}");
        }
    }
}
