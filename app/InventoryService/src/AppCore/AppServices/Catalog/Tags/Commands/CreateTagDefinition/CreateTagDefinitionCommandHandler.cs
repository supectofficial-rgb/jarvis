namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Tags.Commands.CreateTagDefinition;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands.CreateTagDefinition;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateTagDefinitionCommandHandler : CommandHandler<CreateTagDefinitionCommand, CreateTagDefinitionCommandResult>
{
    private readonly ITagCommandRepository _repository;

    public CreateTagDefinitionCommandHandler(ITagCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateTagDefinitionCommandResult>> Handle(CreateTagDefinitionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.TagName))
            return Fail("TagName is required.");

        var normalized = command.TagName.Trim();
        if (await _repository.ExistsByNameAsync(normalized))
            return Fail($"Tag '{normalized}' already exists.");

        var tag = Tag.Create(normalized, command.TagColor);
        await _repository.InsertAsync(tag);
        await _repository.CommitAsync();

        return Ok(new CreateTagDefinitionCommandResult
        {
            TagBusinessKey = tag.BusinessKey.Value,
            TagName = tag.TagName,
            TagColor = tag.TagColor
        });
    }
}
