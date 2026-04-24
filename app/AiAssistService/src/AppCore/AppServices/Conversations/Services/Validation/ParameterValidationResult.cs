namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;

public sealed class ParameterValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingFields { get; set; } = new();
}


