namespace Insurance.InventoryService.AppCore.AppServices.Returns.Commands.CreateReturnRequest;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.CreateReturnRequest;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateReturnRequestCommandHandler : CommandHandler<CreateReturnRequestCommand, Guid>
{
    private readonly IReturnRequestCommandRepository _repository;

    public CreateReturnRequestCommandHandler(IReturnRequestCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateReturnRequestCommand command)
    {
        if (command.OrderRef == Guid.Empty ||
            command.OrderItemRef == Guid.Empty ||
            command.SellerRef == Guid.Empty ||
            command.WarehouseRef == Guid.Empty)
        {
            return Fail("Order, item, seller and warehouse refs are required.");
        }

        if (command.Lines.Count == 0)
            return Fail("Return request must contain at least one line.");

        ReturnRequest request;
        try
        {
            request = ReturnRequest.Create(
                command.OrderRef,
                command.OrderItemRef,
                command.SellerRef,
                command.WarehouseRef,
                command.ReasonCode);

            foreach (var line in command.Lines)
            {
                if (!Enum.TryParse<ReturnDisposition>(line.Disposition, true, out var disposition))
                    disposition = ReturnDisposition.Restock;

                var returnLine = request.AddLine(
                    line.VariantRef,
                    line.Qty,
                    line.UomRef,
                    line.BaseQty,
                    line.BaseUomRef,
                    line.LotBatchNo,
                    line.ExpectedCondition,
                    disposition);

                foreach (var serial in line.Serials)
                    returnLine.AddSerial(serial.SerialRef, serial.SerialNo);
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.InsertAsync(request);
        await _repository.CommitAsync();

        return Ok(request.BusinessKey.Value);
    }
}
