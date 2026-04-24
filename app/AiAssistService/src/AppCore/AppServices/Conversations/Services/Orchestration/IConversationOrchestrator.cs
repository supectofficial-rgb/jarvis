using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

public interface IConversationOrchestrator
{
    Task<AssistantTurnResponse> ProcessTurnAsync(AssistantTurnRequest request, CancellationToken cancellationToken);
}


