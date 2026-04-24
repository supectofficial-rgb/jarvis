namespace Insurance.HubService.AppCore.DomainServices.Conversations;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using OysterFx.AppCore.Shared.DependencyInjections;

public class ConversationAssistantMessageResolverDomainService : IConversationAssistantMessageResolverDomainService, IScopeLifetimeMarker
{
    public string ResolveDisplayMessage(AiAssistantTurnResult turn)
    {
        if (!string.IsNullOrWhiteSpace(turn.Message))
        {
            return turn.Message.Trim();
        }

        var normalizedStatus = string.IsNullOrWhiteSpace(turn.Status)
            ? AiAssistantTurnStatus.MessageOnly
            : turn.Status.Trim().ToLowerInvariant();

        return normalizedStatus switch
        {
            AiAssistantTurnStatus.ClarificationRequired when turn.Suggestions.Count > 0
                => $"Please clarify your request. Suggestions: {string.Join(", ", turn.Suggestions)}",

            AiAssistantTurnStatus.ParamsRequired when turn.MissingFields.Count > 0
                => $"Please provide the missing fields: {string.Join(", ", turn.MissingFields)}",

            AiAssistantTurnStatus.AuthRequired
                => "Authentication is required before continuing.",

            AiAssistantTurnStatus.ConfirmationRequired
                => string.IsNullOrWhiteSpace(turn.ActionName)
                    ? "Please confirm to continue execution."
                    : $"Please confirm action '{turn.ActionName}'.",

            AiAssistantTurnStatus.ReadyToExecute
                => "Request is validated and ready for execution.",

            AiAssistantTurnStatus.ExecutionStarted
                => "Request accepted and execution started.",

            AiAssistantTurnStatus.ExecutionCompleted
                => "Execution completed successfully.",

            AiAssistantTurnStatus.ExecutionFailed
                => "Execution failed.",

            AiAssistantTurnStatus.Cancelled
                => "Request was cancelled.",

            AiAssistantTurnStatus.UnsupportedRequest
                => "I could not resolve your request.",

            _ => "Your message was received. I am preparing the next step."
        };
    }
}


