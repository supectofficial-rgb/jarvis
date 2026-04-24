namespace Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Entities;

public class ConversationSessionReadModel
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset LastActivityUtc { get; set; }
}
