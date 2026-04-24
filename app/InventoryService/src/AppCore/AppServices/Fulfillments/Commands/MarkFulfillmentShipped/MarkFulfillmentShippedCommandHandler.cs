namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Commands.MarkFulfillmentShipped;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentShipped;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class MarkFulfillmentShippedCommandHandler : CommandHandler<MarkFulfillmentShippedCommand, Guid>
{
    private readonly IFulfillmentCommandRepository _repository;

    public MarkFulfillmentShippedCommandHandler(IFulfillmentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(MarkFulfillmentShippedCommand command)
    {
        if (command.FulfillmentBusinessKey == Guid.Empty)
            return Fail("FulfillmentBusinessKey is required.");

        var fulfillment = await _repository.GetByBusinessKeyAsync(command.FulfillmentBusinessKey);
        if (fulfillment is null)
            return Fail("Fulfillment was not found.");

        try
        {
            fulfillment.MarkShipped(command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(fulfillment.BusinessKey.Value);
    }
}
