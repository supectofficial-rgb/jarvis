namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

public interface ISessionService
{
    Task<AssistantSession> GetOrCreateAsync(string? sessionId, string? userId, CancellationToken cancellationToken);
    Task<AssistantSession?> TryGetAsync(string sessionId, CancellationToken cancellationToken);
    Task SaveAsync(AssistantSession session, CancellationToken cancellationToken);
}


