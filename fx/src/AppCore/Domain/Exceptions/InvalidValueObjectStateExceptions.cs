namespace OysterFx.AppCore.Domain.Exceptions;

public class InvalidValueObjectStateExceptions : AggregateStateExceptions
{
    public InvalidValueObjectStateExceptions(string message, string parameter) : base(message, parameter) { }
}