namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.UpdateAttributeOption;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeOption;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateAttributeOptionCommandHandler : CommandHandler<UpdateAttributeOptionCommand, UpdateAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public UpdateAttributeOptionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateAttributeOptionCommandResult>> Handle(UpdateAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        if (command.OptionBusinessKey == Guid.Empty)
            return Fail("OptionBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Value))
            return Fail("Value is required.");

        var normalizedValue = command.Value.Trim();
        var normalizedName = string.IsNullOrWhiteSpace(command.Name)
            ? normalizedValue
            : command.Name.Trim();

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        if (aggregate.DataType != AttributeDataType.Option)
            return Fail("Options are only valid when attribute data type is Option.");

        var duplicate = aggregate.Options.Any(x =>
            x.BusinessKey.Value != command.OptionBusinessKey
            && (string.Equals(x.Value, normalizedValue, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Name, normalizedName, StringComparison.OrdinalIgnoreCase)));
        if (duplicate)
            return Fail("Another option with this name or value already exists.");

        AttributeOption option;
        try
        {
            option = aggregate.UpdateOption(command.OptionBusinessKey, normalizedName, normalizedValue, command.DisplayOrder);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new UpdateAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            OptionBusinessKey = option.BusinessKey.Value,
            Name = option.Name,
            Value = option.Value,
            DisplayOrder = option.DisplayOrder,
            IsActive = option.IsActive
        });
    }
}
