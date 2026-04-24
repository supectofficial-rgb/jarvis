namespace Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Entities;

public class ConversationMessageReadModel
{
    public string MessageId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public int Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }
}
