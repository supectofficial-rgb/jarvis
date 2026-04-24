using OysterFx.AppCore.Shared.DependencyInjections;
namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

public sealed class DialogueStateManager : IDialogueStateManager, ISingletoneLifetimeMarker
{
    public DialogueState MoveToIdle() => DialogueState.Idle;
    public DialogueState MoveToProcessing() => DialogueState.Processing;
    public DialogueState MoveToAuthRequired() => DialogueState.AuthRequired;
    public DialogueState MoveToWaitingForParams() => DialogueState.WaitingForParams;
    public DialogueState MoveToWaitingForConfirmation() => DialogueState.WaitingForConfirmation;
    public DialogueState MoveToReadyToExecute() => DialogueState.ReadyToExecute;
    public DialogueState MoveToExecuting() => DialogueState.Executing;
    public DialogueState MoveToCompleted() => DialogueState.Completed;
    public DialogueState MoveToFailed() => DialogueState.Failed;
    public DialogueState MoveToCancelled() => DialogueState.Cancelled;
}




