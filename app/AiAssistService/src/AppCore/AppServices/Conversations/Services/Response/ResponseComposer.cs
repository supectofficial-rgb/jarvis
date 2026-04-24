using OysterFx.AppCore.Shared.DependencyInjections;
namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;

public sealed class ResponseComposer : IResponseComposer, ISingletoneLifetimeMarker
{
    public AssistantTurnResponse ComposeMessageOnly(string sessionId, string correlationId, string message)
        => new()
        {
            Status = AssistantTurnStatus.MessageOnly,
            Message = message,
            SessionId = sessionId,
            CorrelationId = correlationId
        };

    public AssistantTurnResponse ComposeClarification(string sessionId, string correlationId, string message, List<string> suggestions)
        => new()
        {
            Status = AssistantTurnStatus.ClarificationRequired,
            Message = message,
            SessionId = sessionId,
            CorrelationId = correlationId,
            Suggestions = suggestions
        };

    public AssistantTurnResponse ComposeParamsRequired(string sessionId, string correlationId, string actionName, List<string> missingFields)
        => new()
        {
            Status = AssistantTurnStatus.ParamsRequired,
            Message = "Please provide the missing fields.",
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName,
            MissingFields = missingFields
        };

    public AssistantTurnResponse ComposeAuthRequired(string sessionId, string correlationId)
        => new()
        {
            Status = AssistantTurnStatus.AuthRequired,
            Message = "Authentication is required before continuing.",
            SessionId = sessionId,
            CorrelationId = correlationId
        };

    public AssistantTurnResponse ComposeConfirmationRequired(string sessionId, string correlationId, string actionName)
        => new()
        {
            Status = AssistantTurnStatus.ConfirmationRequired,
            Message = "Please confirm to continue execution.",
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName
        };

    public AssistantTurnResponse ComposeReadyToExecute(string sessionId, string correlationId, string actionName)
        => new()
        {
            Status = AssistantTurnStatus.ReadyToExecute,
            Message = "Request is validated and ready for execution.",
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName
        };

    public AssistantTurnResponse ComposeExecutionStarted(string sessionId, string correlationId, string actionName)
        => new()
        {
            Status = AssistantTurnStatus.ExecutionStarted,
            Message = "Request accepted and execution started.",
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName
        };

    public AssistantTurnResponse ComposeExecutionCompleted(string sessionId, string correlationId, string actionName, object? data)
        => new()
        {
            Status = AssistantTurnStatus.ExecutionCompleted,
            Message = "Execution completed successfully.",
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName,
            Data = data
        };

    public AssistantTurnResponse ComposeExecutionFailed(string sessionId, string correlationId, string actionName, string reason)
        => new()
        {
            Status = AssistantTurnStatus.ExecutionFailed,
            Message = reason,
            SessionId = sessionId,
            CorrelationId = correlationId,
            ActionName = actionName
        };

    public AssistantTurnResponse ComposeCancelled(string sessionId, string correlationId, string message)
        => new()
        {
            Status = AssistantTurnStatus.Cancelled,
            Message = message,
            SessionId = sessionId,
            CorrelationId = correlationId
        };

    public AssistantTurnResponse ComposeUnsupported(string sessionId, string correlationId)
        => new()
        {
            Status = AssistantTurnStatus.UnsupportedRequest,
            Message = "I could not resolve your request.",
            SessionId = sessionId,
            CorrelationId = correlationId
        };
}




