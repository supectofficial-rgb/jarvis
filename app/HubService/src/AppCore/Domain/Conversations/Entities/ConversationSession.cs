namespace Insurance.HubService.AppCore.Domain.Conversations.Entities;

using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class ConversationSession : AggregateRoot
{
    private readonly List<ConversationMessage> _messages = new();

    public ConversationSession(string sessionId, string userId, DateTimeOffset createdAtUtc)
    {
        SessionId = sessionId;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        LastActivityUtc = createdAtUtc;
    }

    public string SessionId { get; }
    public string UserId { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset LastActivityUtc { get; private set; }
    public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();

    public void AppendMessage(
        ConversationMessageRole role,
        string content,
        DateTimeOffset? timestampUtc = null,
        string? messageId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty.", nameof(content));
        }

        var timestamp = timestampUtc ?? DateTimeOffset.UtcNow;
        var normalizedMessageId = string.IsNullOrWhiteSpace(messageId)
            ? Guid.NewGuid().ToString("N")
            : messageId.Trim();

        _messages.Add(new ConversationMessage(
            normalizedMessageId,
            role,
            content.Trim(),
            timestamp));

        LastActivityUtc = timestamp;
    }

    public IReadOnlyList<ConversationMessage> GetMessages(int maxItems)
    {
        if (maxItems <= 0)
        {
            return Array.Empty<ConversationMessage>();
        }

        return _messages
            .TakeLast(maxItems)
            .ToArray();
    }
}
