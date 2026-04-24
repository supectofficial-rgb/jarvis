namespace Insurance.ChatApp.Configuration;

public sealed class HubServiceOptions
{
    public const string SectionName = "HubService";

    public string BaseUrl { get; set; } = "http://localhost:5202";
    public string ConversationBasePath { get; set; } = "api/HubService/Conversation";
    public int TimeoutSeconds { get; set; } = 30;
    public bool IgnoreSslErrors { get; set; }
}
