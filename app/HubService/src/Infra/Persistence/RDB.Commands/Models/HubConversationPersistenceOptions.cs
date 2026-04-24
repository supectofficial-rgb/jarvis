namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Models;

public class HubConversationPersistenceOptions
{
    public const string SectionName = "HubConversation";

    public int SessionIdleTimeoutMinutes { get; set; } = 120;
}
