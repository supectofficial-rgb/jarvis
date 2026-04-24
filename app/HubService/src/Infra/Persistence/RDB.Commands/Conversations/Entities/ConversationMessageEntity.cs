namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;

public class ConversationMessageEntity
{
    public string MessageId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public ConversationMessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }

    public ConversationSessionEntity? Session { get; set; }
}


