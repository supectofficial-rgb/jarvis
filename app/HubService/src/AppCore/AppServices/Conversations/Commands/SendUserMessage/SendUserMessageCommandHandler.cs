namespace Insurance.HubService.AppCore.AppServices.Conversations.Commands.SendUserMessage;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.SendUserMessage;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class SendUserMessageCommandHandler(IConversationOrchestrator orchestrator)
    : CommandHandler<SendUserMessageCommand, ConversationReply>
{
    private readonly IConversationOrchestrator _orchestrator = orchestrator;

    public override async Task<CommandResult<ConversationReply>> Handle(SendUserMessageCommand command)
    {
        var result = await _orchestrator.HandleUserMessageAsync(
            new UserMessageRequest(command.SessionId, command.UserId, command.Text),
            CancellationToken.None);

        return await OkAsync(result);
    }
}





