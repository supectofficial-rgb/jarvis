namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Policy;

public sealed class PolicyCheckResult
{
    public bool Allowed { get; set; }
    public bool AuthRequired { get; set; }
    public string Reason { get; set; } = string.Empty;
}


