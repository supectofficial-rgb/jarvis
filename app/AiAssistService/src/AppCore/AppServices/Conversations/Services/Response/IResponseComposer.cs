namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;

public interface IResponseComposer
{
    AssistantTurnResponse ComposeMessageOnly(string sessionId, string correlationId, string message);
    AssistantTurnResponse ComposeClarification(string sessionId, string correlationId, string message, List<string> suggestions);
    AssistantTurnResponse ComposeParamsRequired(string sessionId, string correlationId, string actionName, List<string> missingFields);
    AssistantTurnResponse ComposeAuthRequired(string sessionId, string correlationId);
    AssistantTurnResponse ComposeConfirmationRequired(string sessionId, string correlationId, string actionName);
    AssistantTurnResponse ComposeReadyToExecute(string sessionId, string correlationId, string actionName);
    AssistantTurnResponse ComposeExecutionStarted(string sessionId, string correlationId, string actionName);
    AssistantTurnResponse ComposeExecutionCompleted(string sessionId, string correlationId, string actionName, object? data);
    AssistantTurnResponse ComposeExecutionFailed(string sessionId, string correlationId, string actionName, string reason);
    AssistantTurnResponse ComposeCancelled(string sessionId, string correlationId, string message);
    AssistantTurnResponse ComposeUnsupported(string sessionId, string correlationId);
}


