namespace OysterFx.AppCore.AppServices.Commands;

public class ValidationError
{
    public ValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
    public string? PropertyName { get; }
    public string? ErrorMessage { get; }

    public string GetMessage() 
        => $"{PropertyName} {ErrorMessage}";
}