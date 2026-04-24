namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

public interface IResultEventHandler
{
    Task HandleAsync(string eventName, string payload, CancellationToken cancellationToken);
}


