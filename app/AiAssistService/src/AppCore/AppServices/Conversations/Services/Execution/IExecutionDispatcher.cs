using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Execution;

public interface IExecutionDispatcher
{
    Task<ExecutionResult> DispatchAsync(ActionMetadata action, Dictionary<string, string?> parameters, string correlationId, CancellationToken cancellationToken);
}


