namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Commands.ProcessTurn;

using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;
using Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.ProcessTurn;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ProcessAssistantTurnCommandHandler : CommandHandler<ProcessAssistantTurnCommand, ProcessAssistantTurnCommandResult>
{
    private readonly IConversationOrchestrator _conversationOrchestrator;

    public ProcessAssistantTurnCommandHandler(IConversationOrchestrator conversationOrchestrator)
    {
        _conversationOrchestrator = conversationOrchestrator;
    }

    public override async Task<CommandResult<ProcessAssistantTurnCommandResult>> Handle(ProcessAssistantTurnCommand command)
    {
        var request = new AssistantTurnRequest
        {
            SessionId = command.SessionId,
            UserId = command.UserId,
            MessageId = command.MessageId,
            Text = command.Text,
            AccessToken = command.AccessToken,
            IsConfirmation = command.IsConfirmation,
            IsRejection = command.IsRejection,
            IsCancellation = command.IsCancellation,
            IsOverride = command.IsOverride,
            PreviewOnly = command.PreviewOnly
        };

        var response = await _conversationOrchestrator.ProcessTurnAsync(request, CancellationToken.None);

        var result = new ProcessAssistantTurnCommandResult
        {
            Status = response.Status,
            Message = response.Message,
            SessionId = response.SessionId,
            CorrelationId = response.CorrelationId,
            ActionName = response.ActionName,
            MissingFields = response.MissingFields,
            Suggestions = response.Suggestions,
            Data = response.Data
        };

        return CommandResult<ProcessAssistantTurnCommandResult>.Success(result);
    }
}


