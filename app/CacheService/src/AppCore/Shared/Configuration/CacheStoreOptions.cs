namespace Insurance.CacheService.AppCore.Shared.Configuration;

public sealed class CacheStoreOptions
{
    public const string SectionName = "Cache";

    public int DefaultExpirationMinutes { get; set; } = 1440;
}
