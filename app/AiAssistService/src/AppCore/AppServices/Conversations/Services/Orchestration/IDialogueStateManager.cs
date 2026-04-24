namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

public interface IDialogueStateManager
{
    DialogueState MoveToIdle();
    DialogueState MoveToProcessing();
    DialogueState MoveToAuthRequired();
    DialogueState MoveToWaitingForParams();
    DialogueState MoveToWaitingForConfirmation();
    DialogueState MoveToReadyToExecute();
    DialogueState MoveToExecuting();
    DialogueState MoveToCompleted();
    DialogueState MoveToFailed();
    DialogueState MoveToCancelled();
}


