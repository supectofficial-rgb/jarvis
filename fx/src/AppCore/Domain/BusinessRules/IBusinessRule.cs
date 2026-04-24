namespace OysterFx.AppCore.Domain.BusinessRules;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}