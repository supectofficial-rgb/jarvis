namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Responses;

public sealed class ErrorResponse
{
    public List<string> Errors { get; set; } = new();
}
