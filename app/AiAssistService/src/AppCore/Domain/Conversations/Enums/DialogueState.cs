namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

public enum DialogueState
{
    Idle = 0,
    Processing = 1,
    WaitingForParams = 2,
    WaitingForConfirmation = 3,
    AuthRequired = 4,
    ReadyToExecute = 5,
    Executing = 6,
    Completed = 7,
    Failed = 8,
    Cancelled = 9
}


