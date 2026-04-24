namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

public sealed record AudioTranscriptionResult(
    string SessionId,
    string CorrelationId,
    string MessageId,
    string Text,
    DateTimeOffset TimestampUtc);
