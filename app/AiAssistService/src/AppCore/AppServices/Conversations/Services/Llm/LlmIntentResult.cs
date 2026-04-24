namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;

public sealed class LlmIntentResult
{
    public string? Action { get; set; }
    public Dictionary<string, string?> Parameters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public double Confidence { get; set; }
    public List<string> MissingFields { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}


