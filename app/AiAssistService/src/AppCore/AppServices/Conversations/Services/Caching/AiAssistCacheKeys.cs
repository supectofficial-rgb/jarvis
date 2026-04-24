namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;

public static class AiAssistCacheKeys
{
    public static string Session(string sessionId) => $"ai-assist--session--{sessionId}";

    public static string UserPermissions(string userId) => $"user--permissions--{userId}";

    public static string ActionCatalog() => "ai-assist--catalog--actions--v1";
}
