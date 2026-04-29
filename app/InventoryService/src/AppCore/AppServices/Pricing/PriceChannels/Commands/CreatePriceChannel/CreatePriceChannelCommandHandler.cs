namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceChannels.Commands.CreatePriceChannel;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Commands.CreatePriceChannel;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreatePriceChannelCommandHandler : CommandHandler<CreatePriceChannelCommand, CreatePriceChannelCommandResult>
{
    private readonly IPriceChannelCommandRepository _repository;

    public CreatePriceChannelCommandHandler(IPriceChannelCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreatePriceChannelCommandResult>> Handle(CreatePriceChannelCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var code = command.Code.Trim();
        if (await _repository.ExistsByCodeAsync(code))
            return Fail($"Price channel code '{code}' already exists.");

        var aggregate = PriceChannel.Create(code, command.Name.Trim());
        await _repository.InsertAsync(aggregate);
        await _repository.CommitAsync();

        return Ok(new CreatePriceChannelCommandResult { PriceChannelBusinessKey = aggregate.BusinessKey.Value });
    }
}
