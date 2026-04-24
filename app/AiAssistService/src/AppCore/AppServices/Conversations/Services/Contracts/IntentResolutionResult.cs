namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;

public sealed class IntentResolutionResult
{
    public string? ActionName { get; set; }
    public double Confidence { get; set; }
    public bool IsAmbiguous { get; set; }
    public Dictionary<string, string?> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> MissingFields { get; set; } = new();
    public string? Notes { get; set; }
}


