namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Entities;

public sealed class SectionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long LanguageId { get; set; }
}
