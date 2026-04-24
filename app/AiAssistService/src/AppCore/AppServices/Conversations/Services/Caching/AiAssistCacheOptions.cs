namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;

public sealed class AiAssistCacheOptions
{
    public const string Key = "AiAssist:Cache";

    public int SessionAbsoluteExpirationMinutes { get; set; } = 120;
    public int CatalogAbsoluteExpirationMinutes { get; set; } = 60;
}
