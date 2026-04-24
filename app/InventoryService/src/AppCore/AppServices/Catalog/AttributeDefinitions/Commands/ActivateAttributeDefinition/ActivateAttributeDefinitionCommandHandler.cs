namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.ActivateAttributeDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.ActivateAttributeDefinition;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateAttributeDefinitionCommandHandler
    : CommandHandler<ActivateAttributeDefinitionCommand, ActivateAttributeDefinitionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public ActivateAttributeDefinitionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<ActivateAttributeDefinitionCommandResult>> Handle(ActivateAttributeDefinitionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        aggregate.Activate();
        await _repository.CommitAsync();

        return Ok(new ActivateAttributeDefinitionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}
