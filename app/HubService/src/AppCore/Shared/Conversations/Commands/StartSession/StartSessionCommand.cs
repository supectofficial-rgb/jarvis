namespace Insurance.HubService.AppCore.Shared.Conversations.Commands.StartSession;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Shared.Commands;

public class StartSessionCommand : ICommand<ConversationSessionStarted>
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
}


