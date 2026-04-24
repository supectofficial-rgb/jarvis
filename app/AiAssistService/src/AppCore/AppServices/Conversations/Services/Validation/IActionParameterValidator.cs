using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;

public interface IActionParameterValidator
{
    ParameterValidationResult Validate(ActionMetadata action, Dictionary<string, string?> parameters);
}


