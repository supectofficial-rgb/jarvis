namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Commands.UpdatePriceChannel;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.UpdatePriceChannel;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdatePriceChannelCommandHandler : CommandHandler<UpdatePriceChannelCommand, UpdatePriceChannelCommandResult>
{
    private readonly IPriceChannelCommandRepository _repository;

    public UpdatePriceChannelCommandHandler(IPriceChannelCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdatePriceChannelCommandResult>> Handle(UpdatePriceChannelCommand command)
    {
        if (command.PriceChannelBusinessKey == Guid.Empty)
            return Fail("PriceChannelBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.PriceChannelBusinessKey);
        if (aggregate is null)
            return Fail("Price channel was not found.");

        var code = command.Code.Trim();
        if (!string.Equals(aggregate.Code, code, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(code, command.PriceChannelBusinessKey))
        {
            return Fail($"Price channel code '{code}' already exists.");
        }

        aggregate.ChangeCode(code);
        aggregate.Rename(command.Name.Trim());
        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        await _repository.CommitAsync();
        return Ok(new UpdatePriceChannelCommandResult { PriceChannelBusinessKey = aggregate.BusinessKey.Value });
    }
}
