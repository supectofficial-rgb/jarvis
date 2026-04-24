namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken);
}


