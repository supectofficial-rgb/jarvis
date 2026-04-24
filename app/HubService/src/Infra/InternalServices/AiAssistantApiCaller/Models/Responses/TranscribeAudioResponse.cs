namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Responses;

public sealed class TranscribeAudioResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
