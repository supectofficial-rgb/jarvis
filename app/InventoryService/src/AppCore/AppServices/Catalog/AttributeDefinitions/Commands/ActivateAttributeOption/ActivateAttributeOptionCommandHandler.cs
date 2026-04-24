namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.ActivateAttributeOption;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeOption;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateAttributeOptionCommandHandler : CommandHandler<ActivateAttributeOptionCommand, ActivateAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public ActivateAttributeOptionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateAttributeOptionCommandResult>> Handle(ActivateAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty || command.OptionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey and OptionBusinessKey are required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var option = aggregate.Options.FirstOrDefault(x => x.BusinessKey.Value == command.OptionBusinessKey);
        if (option is null)
            return Fail("Attribute option was not found.");

        try
        {
            aggregate.SetOptionActive(command.OptionBusinessKey, true);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new ActivateAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            OptionBusinessKey = option.BusinessKey.Value,
            IsActive = aggregate.Options.First(x => x.BusinessKey.Value == command.OptionBusinessKey).IsActive
        });
    }
}
