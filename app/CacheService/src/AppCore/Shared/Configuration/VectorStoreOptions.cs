namespace Insurance.CacheService.AppCore.Shared.Configuration;

public sealed class VectorStoreOptions
{
    public const string SectionName = "Vector";

    public string DefaultIndexName { get; set; } = "ai-assist--vector-index";
    public string DefaultPrefix { get; set; } = "ai-assist--vector--endpoint:";
    public int DefaultDimension { get; set; } = 1536;
    public int DefaultTopK { get; set; } = 5;
}
