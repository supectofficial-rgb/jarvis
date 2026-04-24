namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Requests;

public sealed class ProcessAssistantTurnRequest
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string Text { get; set; } = string.Empty;
}
