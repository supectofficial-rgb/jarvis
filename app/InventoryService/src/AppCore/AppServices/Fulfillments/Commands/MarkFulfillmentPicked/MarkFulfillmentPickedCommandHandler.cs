namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Commands.MarkFulfillmentPicked;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentPicked;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class MarkFulfillmentPickedCommandHandler : CommandHandler<MarkFulfillmentPickedCommand, Guid>
{
    private readonly IFulfillmentCommandRepository _repository;

    public MarkFulfillmentPickedCommandHandler(IFulfillmentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(MarkFulfillmentPickedCommand command)
    {
        if (command.FulfillmentBusinessKey == Guid.Empty)
            return Fail("FulfillmentBusinessKey is required.");

        var fulfillment = await _repository.GetByBusinessKeyAsync(command.FulfillmentBusinessKey);
        if (fulfillment is null)
            return Fail("Fulfillment was not found.");

        try
        {
            fulfillment.MarkPicked(command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(fulfillment.BusinessKey.Value);
    }
}
