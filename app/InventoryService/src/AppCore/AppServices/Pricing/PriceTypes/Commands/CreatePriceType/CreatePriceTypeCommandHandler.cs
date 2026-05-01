namespace Insurance.InventoryService.AppCore.AppServices.Pricing.PriceTypes.Commands.CreatePriceType;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Commands.CreatePriceType;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreatePriceTypeCommandHandler : CommandHandler<CreatePriceTypeCommand, CreatePriceTypeCommandResult>
{
    private readonly IPriceTypeCommandRepository _repository;

    public CreatePriceTypeCommandHandler(IPriceTypeCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreatePriceTypeCommandResult>> Handle(CreatePriceTypeCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        try
        {
            var code = command.Code.Trim();
            if (await _repository.ExistsByCodeAsync(code))
                return Fail($"Price type code '{code}' already exists.");

            var aggregate = PriceType.Create(code, command.Name.Trim());
            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreatePriceTypeCommandResult { PriceTypeBusinessKey = aggregate.BusinessKey.Value });
        }
        catch (Exception ex)
        {
            return Fail($"Creating price type failed: {ex.Message}");
        }
    }
}
