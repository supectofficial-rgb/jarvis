namespace Insurance.PageRuntimeService.AppCore.Shared.Languages.Commands.CreateLanguage;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateLanguageCommand : ICommand<CreateLanguageCommandResult>
{
    public string Title { get; set; } = string.Empty;
}

public sealed class CreateLanguageCommandResult
{
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
}

