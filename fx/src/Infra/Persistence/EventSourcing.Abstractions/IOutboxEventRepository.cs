namespace OysterFx.Infra.Persistence.EventSourcing.Abstractions;

public interface IOutboxEventRepository
{
    public IEnumerable<OutboxEvent> ReadEvents(int size = 10);
    public bool MarkAsRead(IEnumerable<OutboxEvent> events);
}