namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;

public static class AssistantTurnStatus
{
    public const string MessageOnly = "message_only";
    public const string ClarificationRequired = "clarification_required";
    public const string ParamsRequired = "params_required";
    public const string AuthRequired = "auth_required";
    public const string ConfirmationRequired = "confirmation_required";
    public const string ReadyToExecute = "ready_to_execute";
    public const string ExecutionStarted = "execution_started";
    public const string ExecutionCompleted = "execution_completed";
    public const string ExecutionFailed = "execution_failed";
    public const string Cancelled = "cancelled";
    public const string UnsupportedRequest = "unsupported_request";
}


