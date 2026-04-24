namespace OysterFx.Infra.EventBus.Outbox;

public sealed class OutboxDispatcherOptions
{
    public const string SectionName = "OutboxEventSourcing";

    public bool Enabled { get; init; } = true;
    public int SendInterval { get; init; } = 1000;
    public int ExceptionInterval { get; init; } = 10_000;
    public int SendCount { get; init; } = 100;
    public string? ApplicationName { get; init; }
}
