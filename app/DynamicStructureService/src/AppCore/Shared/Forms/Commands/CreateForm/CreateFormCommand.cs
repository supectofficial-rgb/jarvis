namespace Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.CreateForm;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateFormCommand : ICommand<CreateFormCommandResult>
{
    public string Title { get; set; } = string.Empty;
    public long? FormTypeId { get; set; }
    public long? UserId { get; set; }
}

public sealed class CreateFormCommandResult
{
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
}

