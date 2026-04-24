namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

public sealed record UserMessageRequest(
    string? SessionId,
    string? UserId,
    string Text);
