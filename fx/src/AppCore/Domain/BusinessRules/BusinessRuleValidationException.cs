namespace OysterFx.AppCore.Domain.BusinessRules;

public class BusinessRuleValidationException : Exception
{
    public IBusinessRule? BrokenRule { get; }

    public BusinessRuleValidationException(IBusinessRule? brokenRule)
        : base(brokenRule?.Message)
    {
        BrokenRule = brokenRule;
    }

    public BusinessRuleValidationException(string message)
        : base(message)
    {
    }
}