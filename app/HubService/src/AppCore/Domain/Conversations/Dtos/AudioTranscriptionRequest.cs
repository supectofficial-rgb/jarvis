namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

public sealed record AudioTranscriptionRequest(
    string? SessionId,
    string? UserId,
    string AudioBase64,
    string? Extension,
    string? MessageId);
