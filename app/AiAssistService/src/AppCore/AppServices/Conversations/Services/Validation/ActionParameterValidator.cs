using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;

public sealed class ActionParameterValidator : IActionParameterValidator, ISingletoneLifetimeMarker
{
    public ParameterValidationResult Validate(ActionMetadata action, Dictionary<string, string?> parameters)
    {
        var missing = new List<string>();

        foreach (var requiredParam in action.RequiredParams)
        {
            if (!parameters.TryGetValue(requiredParam, out var value) || string.IsNullOrWhiteSpace(value))
            {
                missing.Add(requiredParam);
            }
        }

        return new ParameterValidationResult
        {
            IsValid = missing.Count == 0,
            MissingFields = missing
        };
    }
}




