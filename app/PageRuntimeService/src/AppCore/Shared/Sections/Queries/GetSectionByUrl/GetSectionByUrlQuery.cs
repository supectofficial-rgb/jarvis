namespace Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries.GetSectionByUrl;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetSectionByUrlQuery : IQuery<GetSectionByUrlQueryResult>
{
    public string Url { get; set; } = string.Empty;
    public string? Lang { get; set; }
}

public sealed class GetSectionByUrlQueryResult
{
    public Guid BusinessKey { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}

