namespace Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands.CreateSection;

using OysterFx.AppCore.Shared.Commands;

public sealed class CreateSectionCommand : ICommand<CreateSectionCommandResult>
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public long LanguageId { get; set; }
    public string? Description { get; set; }
}

public sealed class CreateSectionCommandResult
{
    public Guid BusinessKey { get; set; }
    public string Url { get; set; } = string.Empty;
}

