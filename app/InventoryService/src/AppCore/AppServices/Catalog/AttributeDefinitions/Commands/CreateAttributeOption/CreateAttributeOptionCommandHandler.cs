namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.CreateAttributeOption;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeOption;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateAttributeOptionCommandHandler : CommandHandler<CreateAttributeOptionCommand, CreateAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public CreateAttributeOptionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateAttributeOptionCommandResult>> Handle(CreateAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

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

        var option = aggregate.AddOption(normalizedName, normalizedValue, command.DisplayOrder);
        await _repository.CommitAsync();

        return Ok(new CreateAttributeOptionCommandResult
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
