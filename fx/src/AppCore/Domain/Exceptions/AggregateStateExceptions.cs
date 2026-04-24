namespace OysterFx.AppCore.Domain.Exceptions;

public class AggregateStateExceptions : Exception
{
    public string Parameter { get; set; }
    public AggregateStateExceptions(string message, string parameter) : base(message)
    {
        Parameter = parameter;
    }
}