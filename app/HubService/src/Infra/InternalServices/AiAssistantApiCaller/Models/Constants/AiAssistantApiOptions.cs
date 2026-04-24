namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Constants;

public sealed class AiAssistantApiOptions
{
    public const string Key = "AiAssistant";

    public bool Enabled { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
    public string ProcessPath { get; set; } = "api/AiAssistService/assistant/turn";
    public string TranscribePath { get; set; } = "api/stt/transcribe";
    public int TimeoutMs { get; set; } = 15000;
}
