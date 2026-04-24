namespace OysterFx.Infra.EventBus.RabbitMqBroker.Consumers.Convention;

internal sealed record EventConsumerRegistration(Type PayloadType, string EventType);
