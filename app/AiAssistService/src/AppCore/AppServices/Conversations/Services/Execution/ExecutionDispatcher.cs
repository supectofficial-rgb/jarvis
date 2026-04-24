using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Execution;

public sealed class ExecutionDispatcher : IExecutionDispatcher, IScopeLifetimeMarker
{
    private readonly IMessagePublisher _publisher;

    public ExecutionDispatcher(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<ExecutionResult> DispatchAsync(ActionMetadata action, Dictionary<string, string?> parameters, string correlationId, CancellationToken cancellationToken)
    {
        try
        {
            if (action.IsAsync)
            {
                await _publisher.PublishAsync("assistant.action.requested", new
                {
                    correlationId,
                    action = action.ActionName,
                    parameters
                }, cancellationToken);

                return new ExecutionResult
                {
                    Success = true,
                    IsAsync = true,
                    Message = "Execution started."
                };
            }

            return new ExecutionResult
            {
                Success = true,
                IsAsync = false,
                Message = $"Action '{action.ActionName}' executed.",
                Payload = new { correlationId, action = action.ActionName, parameters }
            };
        }
        catch (Exception ex)
        {
            return new ExecutionResult
            {
                Success = false,
                IsAsync = action.IsAsync,
                Message = $"Execution failed: {ex.Message}"
            };
        }
    }
}



