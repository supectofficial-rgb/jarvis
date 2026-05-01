namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Commands.UpdatePriceType;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.UpdatePriceType;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdatePriceTypeCommandHandler : CommandHandler<UpdatePriceTypeCommand, UpdatePriceTypeCommandResult>
{
    private readonly IPriceTypeCommandRepository _repository;

    public UpdatePriceTypeCommandHandler(IPriceTypeCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdatePriceTypeCommandResult>> Handle(UpdatePriceTypeCommand command)
    {
        if (command.PriceTypeBusinessKey == Guid.Empty)
            return Fail("PriceTypeBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var aggregate = await _repository.GetByBusinessKeyAsync(command.PriceTypeBusinessKey);
            if (aggregate is null)
                return Fail("Price type was not found.");

            var code = command.Code.Trim();
            if (!string.Equals(aggregate.Code, code, StringComparison.OrdinalIgnoreCase)
                && await _repository.ExistsByCodeAsync(code, command.PriceTypeBusinessKey))
            {
                return Fail($"Price type code '{code}' already exists.");
            }

            aggregate.ChangeCode(code);
            aggregate.Rename(command.Name.Trim());
            if (command.IsActive)
                aggregate.Activate();
            else
                aggregate.Deactivate();

            await _repository.CommitAsync();
            return Ok(new UpdatePriceTypeCommandResult { PriceTypeBusinessKey = aggregate.BusinessKey.Value });
        }
        catch (Exception ex)
        {
            return Fail($"Updating price type failed: {ex.Message}");
        }
    }
}
