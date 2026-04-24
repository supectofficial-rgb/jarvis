namespace Insurance.InventoryService.AppCore.AppServices.SerialItems.Commands.UpdateSerialItemStatus;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.UpdateSerialItemStatus;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateSerialItemStatusCommandHandler : CommandHandler<UpdateSerialItemStatusCommand, UpdateSerialItemStatusCommandResult>
{
    private readonly ISerialItemCommandRepository _repository;

    public UpdateSerialItemStatusCommandHandler(ISerialItemCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateSerialItemStatusCommandResult>> Handle(UpdateSerialItemStatusCommand command)
    {
        if (command.SerialItemBusinessKey == Guid.Empty)
            return Fail("SerialItemBusinessKey is required.");

        if (!Enum.TryParse<SerialItemStatus>(command.Status, true, out var targetStatus))
            return Fail("Status is invalid.");

        var serialItem = await _repository.GetByBusinessKeyAsync(command.SerialItemBusinessKey);
        if (serialItem is null)
            return Fail("Serial item was not found.");

        try
        {
            switch (targetStatus)
            {
                case SerialItemStatus.Available:
                    if (serialItem.Status == SerialItemStatus.Quarantine)
                    {
                        serialItem.ReleaseFromQuarantine(command.QualityStatusRef ?? serialItem.QualityStatusRef);
                    }
                    else if (serialItem.Status == SerialItemStatus.Reserved)
                    {
                        serialItem.ReleaseReservation();
                    }
                    else
                    {
                        serialItem.ReturnToAvailable(
                            command.WarehouseRef ?? serialItem.WarehouseRef,
                            command.LocationRef ?? serialItem.LocationRef,
                            command.QualityStatusRef ?? serialItem.QualityStatusRef,
                            command.LotBatchNo ?? serialItem.LotBatchNo);
                    }
                    break;
                case SerialItemStatus.Reserved:
                    serialItem.Reserve();
                    break;
                case SerialItemStatus.Issued:
                    if (!command.LastTransactionRef.HasValue || command.LastTransactionRef.Value == Guid.Empty)
                        return Fail("LastTransactionRef is required for Issued status.");

                    serialItem.Issue(BusinessKey.FromGuid(command.LastTransactionRef.Value));
                    break;
                case SerialItemStatus.Returned:
                    serialItem.MarkReturned();
                    break;
                case SerialItemStatus.Scrapped:
                    serialItem.Scrap();
                    break;
                case SerialItemStatus.Quarantine:
                    serialItem.Quarantine();
                    break;
                default:
                    return Fail("Unsupported status transition.");
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new UpdateSerialItemStatusCommandResult
        {
            SerialItemBusinessKey = serialItem.BusinessKey.Value,
            Status = serialItem.Status.ToString()
        });
    }
}
