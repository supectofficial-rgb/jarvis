using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;

public sealed class TurnContext
{
    public required AssistantTurnRequest Request { get; init; }
    public required AssistantSession Session { get; init; }
    public required string CorrelationId { get; init; }
}


