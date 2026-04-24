namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;

public interface IIntentLlmService
{
    Task<LlmIntentResult> ResolveAsync(IntentLlmRequest request, CancellationToken cancellationToken);
}


