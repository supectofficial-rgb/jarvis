namespace OysterFx.Infra.EventBus.Abstractions;

using System.Text.Json;

public sealed class JsonMessageSerializer : IMessageSerializer
{
    public string Serialize<T>(T value) => JsonSerializer.Serialize(value);
    public T Deserialize<T>(string payload) => JsonSerializer.Deserialize<T>(payload)!;
}