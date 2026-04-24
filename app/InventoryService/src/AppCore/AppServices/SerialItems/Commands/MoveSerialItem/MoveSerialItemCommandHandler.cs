namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Commands.MoveSerialItem;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.MoveSerialItem;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class MoveSerialItemCommandHandler : CommandHandler<MoveSerialItemCommand, MoveSerialItemCommandResult>
{
    private readonly ISerialItemCommandRepository _repository;

    public MoveSerialItemCommandHandler(ISerialItemCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<MoveSerialItemCommandResult>> Handle(MoveSerialItemCommand command)
    {
        if (command.SerialItemBusinessKey == Guid.Empty)
            return Fail("SerialItemBusinessKey is required.");

        if (command.WarehouseRef == Guid.Empty || command.LocationRef == Guid.Empty || command.QualityStatusRef == Guid.Empty)
            return Fail("WarehouseRef, LocationRef and QualityStatusRef are required.");

        var serialItem = await _repository.GetByBusinessKeyAsync(command.SerialItemBusinessKey);
        if (serialItem is null)
            return Fail("Serial item was not found.");

        try
        {
            serialItem.Move(command.WarehouseRef, command.LocationRef, command.QualityStatusRef, command.LotBatchNo);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new MoveSerialItemCommandResult
        {
            SerialItemBusinessKey = serialItem.BusinessKey.Value,
            WarehouseRef = serialItem.WarehouseRef,
            LocationRef = serialItem.LocationRef,
            QualityStatusRef = serialItem.QualityStatusRef
        });
    }
}
