namespace OysterFx.Infra.EventBus.Abstractions;

public interface IMessageSerializer
{
    string Serialize<T>(T value);
    T Deserialize<T>(string payload);
}