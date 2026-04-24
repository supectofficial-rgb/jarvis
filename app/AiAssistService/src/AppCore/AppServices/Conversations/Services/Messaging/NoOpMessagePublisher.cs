using OysterFx.AppCore.Shared.DependencyInjections;
namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

public sealed class NoOpMessagePublisher : IMessagePublisher, ISingletoneLifetimeMarker
{
    public Task PublishAsync(string eventName, object payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}




