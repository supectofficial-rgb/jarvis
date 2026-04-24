namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Policy;

public interface IPolicyChecker
{
    Task<PolicyCheckResult> PreCheckAsync(string? userId, string permissionKey, string? accessToken, CancellationToken cancellationToken);
    Task<PolicyCheckResult> FinalCheckAsync(string? userId, string permissionKey, string? accessToken, CancellationToken cancellationToken);
}


