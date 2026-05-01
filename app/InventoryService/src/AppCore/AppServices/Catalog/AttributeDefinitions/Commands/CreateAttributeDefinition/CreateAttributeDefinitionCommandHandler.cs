namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.CreateAttributeDefinition;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.CreateAttributeDefinition;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateAttributeDefinitionCommandHandler
    : CommandHandler<CreateAttributeDefinitionCommand, CreateAttributeDefinitionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;

    public CreateAttributeDefinitionCommandHandler(IAttributeDefinitionCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateAttributeDefinitionCommandResult>> Handle(CreateAttributeDefinitionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (!Enum.TryParse<AttributeDataType>(command.DataType, true, out var dataType))
            return Fail($"Unsupported data type '{command.DataType}'.");

        if (!Enum.TryParse<AttributeScope>(command.Scope, true, out var scope))
            return Fail($"Unsupported scope '{command.Scope}'.");

        var normalizedCode = command.Code.Trim();
        bool codeExists;
        try
        {
            codeExists = await _repository.ExistsByCodeAsync(normalizedCode);
        }
        catch (Exception ex)
        {
            return Fail($"Checking attribute definition code failed: {GetExceptionMessage(ex)}");
        }

        if (codeExists)
            return Fail($"Attribute definition code '{normalizedCode}' already exists.");

        var options = (command.Options ?? new List<CreateAttributeOptionItem>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x =>
            {
                var normalizedValue = x.Value.Trim();
                var normalizedName = string.IsNullOrWhiteSpace(x.Name) ? normalizedValue : x.Name.Trim();
                return new OptionInput(normalizedName, normalizedValue, x.DisplayOrder);
            })
            .ToList();

        if (options.GroupBy(x => x.Value, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1))
            return Fail("Duplicate option values are not allowed.");

        if (options.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1))
            return Fail("Duplicate option names are not allowed.");

        if (dataType != AttributeDataType.Option && options.Count > 0)
            return Fail("Options are only allowed when DataType is Option.");

        AttributeDefinition aggregate;
        try
        {
            aggregate = AttributeDefinition.Create(normalizedCode, command.Name.Trim(), dataType, scope);
            foreach (var option in options)
                aggregate.AddOption(option.Name, option.Value, option.DisplayOrder);
        }
        catch (Exception ex)
        {
            return Fail($"Preparing attribute definition failed: {GetExceptionMessage(ex)}");
        }

        try
        {
            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();
        }
        catch (Exception ex)
        {
            return Fail($"Creating attribute definition failed: {GetExceptionMessage(ex)}");
        }

        return Ok(new CreateAttributeDefinitionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            DataType = aggregate.DataType.ToString(),
            Scope = aggregate.Scope.ToString(),
            IsActive = aggregate.IsActive
        });
    }

    private static string GetExceptionMessage(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
                messages.Add(current.Message);
        }

        return string.Join(" | ", messages.Distinct());
    }

    private sealed record OptionInput(string Name, string Value, int DisplayOrder);
}
