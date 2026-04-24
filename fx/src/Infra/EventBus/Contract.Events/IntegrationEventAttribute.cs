namespace OysterFx.Infra.EventBus.Contract.Events;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IntegrationEventAttribute : Attribute
{
    public IntegrationEventAttribute(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty.", nameof(eventType));

        EventType = eventType;
    }

    public string EventType { get; }
}
