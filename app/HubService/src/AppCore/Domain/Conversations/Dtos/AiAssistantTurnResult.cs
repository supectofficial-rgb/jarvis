namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

public sealed record AiAssistantTurnResult(
    string Status,
    string Message,
    string SessionId,
    string CorrelationId,
    string? ActionName,
    IReadOnlyList<string> MissingFields,
    IReadOnlyList<string> Suggestions,
    object? Data);
