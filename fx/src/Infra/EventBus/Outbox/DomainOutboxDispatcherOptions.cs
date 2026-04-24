namespace OysterFx.Infra.EventBus.Outbox;

public sealed class DomainOutboxDispatcherOptions
{
    public const string SectionName = "DomainOutboxDispatcher";

    public bool Enabled { get; set; } = true;
    public int PollingIntervalMs { get; set; } = 1000;
    public int ErrorIntervalMs { get; set; } = 5000;
    public int BatchSize { get; set; } = 100;
}
