namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.AddAttributeOption;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.AddAttributeOption;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class AddAttributeOptionCommandHandler
    : CommandHandler<AddAttributeOptionCommand, AddAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public AddAttributeOptionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<AddAttributeOptionCommandResult>> Handle(AddAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Value))
            return Fail("Option value is required.");

        var normalizedValue = command.Value.Trim();
        var normalizedName = string.IsNullOrWhiteSpace(command.Name)
            ? normalizedValue
            : command.Name.Trim();

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        if (aggregate.DataType != AttributeDataType.Option)
            return Fail("Options are only valid when attribute data type is Option.");

        var option = aggregate.AddOption(normalizedName, normalizedValue, command.DisplayOrder);
        await _repository.CommitAsync();

        return Ok(new AddAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            Name = option.Name,
            Value = option.Value,
            DisplayOrder = option.DisplayOrder,
            IsActive = option.IsActive
        });
    }
}
