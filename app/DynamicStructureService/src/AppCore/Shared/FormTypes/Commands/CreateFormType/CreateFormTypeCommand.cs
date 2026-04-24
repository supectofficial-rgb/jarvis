namespace Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands.CreateFormType;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateFormTypeCommand : ICommand<CreateFormTypeCommandResult>
{
    public string Title { get; set; } = string.Empty;
}

public sealed class CreateFormTypeCommandResult
{
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
}

