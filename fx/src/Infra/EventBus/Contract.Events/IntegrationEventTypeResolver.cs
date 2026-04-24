namespace OysterFx.Infra.EventBus.Contract.Events;

public static class IntegrationEventTypeResolver
{
    public static string Resolve<T>(string? explicitEventType = null) => Resolve(typeof(T), explicitEventType);

    public static string Resolve(Type payloadType, string? explicitEventType = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitEventType))
            return explicitEventType.Trim();

        var attr = payloadType.GetCustomAttributes(typeof(IntegrationEventAttribute), inherit: false)
            .OfType<IntegrationEventAttribute>()
            .FirstOrDefault();

        return !string.IsNullOrWhiteSpace(attr?.EventType)
            ? attr.EventType
            : payloadType.Name;
    }
}
