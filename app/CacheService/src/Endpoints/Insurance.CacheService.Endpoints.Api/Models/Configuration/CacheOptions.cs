namespace Insurance.CacheService.Endpoints.Api.Models.Configuration;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public int DefaultExpirationMinutes { get; set; } = 1440;
}