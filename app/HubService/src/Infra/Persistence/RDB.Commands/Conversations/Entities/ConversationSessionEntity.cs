namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;

public class ConversationSessionEntity
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset LastActivityUtc { get; set; }

    public ICollection<ConversationMessageEntity> Messages { get; set; } = new List<ConversationMessageEntity>();
}
