namespace Insurance.HubService.AppCore.Domain.Conversations.Dtos;

using Insurance.HubService.AppCore.Domain.Conversations.Entities;

public sealed record ConversationReply(
    string SessionId,
    string AssistantMessage,
    AiAssistantTurnResult Assistant,
    IReadOnlyList<ConversationMessage> History,
    DateTimeOffset RespondedAtUtc);
