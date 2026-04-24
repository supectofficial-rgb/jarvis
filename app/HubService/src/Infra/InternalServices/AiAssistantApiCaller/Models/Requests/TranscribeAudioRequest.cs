namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Requests;

public sealed class TranscribeAudioRequest
{
    public string? SessionId { get; set; }
    public string? MessageId { get; set; }
    public string AudioBase64 { get; set; } = string.Empty;
    public string? Extension { get; set; }
}
