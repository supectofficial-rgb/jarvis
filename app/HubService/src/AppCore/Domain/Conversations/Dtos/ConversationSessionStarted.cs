namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

public sealed record ConversationSessionStarted(
    string SessionId,
    string UserId,
    DateTimeOffset StartedAtUtc);
