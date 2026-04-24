namespace OysterFx.AppCore.Domain.BusinessRules;

public class SpecificationResult
{
    public bool IsSatisfied { get; }
    public string? Message { get; }

    private SpecificationResult(bool isSatisfied, string? message)
    {
        IsSatisfied = isSatisfied;
        Message = message;
    }

    public static SpecificationResult Success() => new(true, null);
    public static SpecificationResult Fail(string message) => new(false, message);
}