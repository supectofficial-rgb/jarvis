namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime.Entities;

public sealed class LanguageReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Title { get; set; } = string.Empty;
}
