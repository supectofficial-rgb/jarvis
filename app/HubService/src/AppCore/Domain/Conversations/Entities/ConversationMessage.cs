namespace Insurance.HubService.AppCore.Domain.Conversations.Entities;

using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class ConversationMessage : Aggregate
{
    public string MessageId { get; private set; }
    public ConversationMessageRole Role { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset TimestampUtc { get; private set; }

    private ConversationMessage()
    {
        MessageId = string.Empty;
        Content = string.Empty;
    }

    public ConversationMessage(string messageId, ConversationMessageRole role, string content, DateTimeOffset timestampUtc)
    {
        MessageId = messageId;
        Role = role;
        Content = content;
        TimestampUtc = timestampUtc;
    }
}
