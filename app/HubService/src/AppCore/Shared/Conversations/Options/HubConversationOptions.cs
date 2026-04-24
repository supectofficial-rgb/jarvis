namespace Insurance.HubService.AppCore.Shared.Conversations.Options;

public sealed class HubConversationOptions
{
    public const string SectionName = "HubConversation";

    public int MaxHistoryMessages { get; set; } = 20;
    public int SessionIdleTimeoutMinutes { get; set; } = 120;
}
