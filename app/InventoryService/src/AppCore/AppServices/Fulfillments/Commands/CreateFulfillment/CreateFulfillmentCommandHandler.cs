namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Commands.CreateFulfillment;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.CreateFulfillment;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateFulfillmentCommandHandler : CommandHandler<CreateFulfillmentCommand, Guid>
{
    private readonly IFulfillmentCommandRepository _repository;

    public CreateFulfillmentCommandHandler(IFulfillmentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateFulfillmentCommand command)
    {
        if (command.OrderRef == Guid.Empty ||
            command.SellerRef == Guid.Empty ||
            command.WarehouseRef == Guid.Empty ||
            command.ChannelRef == Guid.Empty)
        {
            return Fail("Order, seller, warehouse and channel refs are required.");
        }

        if (command.Lines.Count == 0)
            return Fail("Fulfillment must contain at least one line.");

        Fulfillment fulfillment;
        try
        {
            fulfillment = Fulfillment.Create(
                command.OrderRef,
                command.SellerRef,
                command.WarehouseRef,
                command.ChannelRef);

            foreach (var line in command.Lines)
            {
                var fulfillmentLine = fulfillment.AddLine(
                    line.VariantRef,
                    line.Qty,
                    line.UomRef,
                    line.BaseQty,
                    line.BaseUomRef,
                    line.SourceLocationRef,
                    line.LotBatchNo,
                    line.ReservationAllocationRef);

                foreach (var serial in line.Serials)
                    fulfillmentLine.AddSerial(serial.SerialRef, serial.SerialNo);
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.InsertAsync(fulfillment);
        await _repository.CommitAsync();

        return Ok(fulfillment.BusinessKey.Value);
    }
}
