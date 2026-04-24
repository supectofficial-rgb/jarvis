using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public sealed class IntentResolver : IIntentResolver, ISingletoneLifetimeMarker
{
    public IntentResolutionResult Resolve(IReadOnlyList<ActionCandidate> candidates, LlmIntentResult llmResult)
    {
        var topCandidate = candidates.OrderByDescending(x => x.Score).FirstOrDefault();

        var actionName = string.IsNullOrWhiteSpace(llmResult.Action)
            ? topCandidate?.ActionName
            : llmResult.Action;

        var confidence = Math.Max(llmResult.Confidence, topCandidate?.Score ?? 0);

        return new IntentResolutionResult
        {
            ActionName = actionName,
            Confidence = confidence,
            IsAmbiguous = confidence < 0.6,
            Parameters = new Dictionary<string, string?>(llmResult.Parameters, StringComparer.OrdinalIgnoreCase),
            MissingFields = llmResult.MissingFields.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            Notes = llmResult.Notes
        };
    }
}




