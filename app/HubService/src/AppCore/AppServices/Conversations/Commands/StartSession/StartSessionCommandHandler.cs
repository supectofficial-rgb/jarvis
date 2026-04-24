namespace Insurance.HubService.AppCore.AppServices.Conversations.Commands.StartSession;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.StartSession;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class StartSessionCommandHandler(IConversationOrchestrator orchestrator)
    : CommandHandler<StartSessionCommand, ConversationSessionStarted>
{
    private readonly IConversationOrchestrator _orchestrator = orchestrator;

    public override async Task<CommandResult<ConversationSessionStarted>> Handle(StartSessionCommand command)
    {
        var result = await _orchestrator.StartSessionAsync(command.SessionId, command.UserId, CancellationToken.None);
        return await OkAsync(result);
    }
}
