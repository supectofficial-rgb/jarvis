namespace Insurance.HubService.AppCore.Shared.Conversations.Commands.SendUserMessage;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Shared.Commands;

public class SendUserMessageCommand : ICommand<ConversationReply>
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string Text { get; set; } = string.Empty;
}


