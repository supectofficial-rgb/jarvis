using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public sealed class RuleBasedCandidateProvider : IRuleBasedCandidateProvider, ISingletoneLifetimeMarker
{
    public IReadOnlyList<ActionCandidate> Resolve(string normalizedText, IReadOnlyList<ActionMetadata> catalog)
    {
        var input = normalizedText ?? string.Empty;
        var result = new List<ActionCandidate>();

        foreach (var action in catalog)
        {
            var score = 0.0;
            if (input.Contains(action.ActionName, StringComparison.OrdinalIgnoreCase))
            {
                score = Math.Max(score, 0.95);
            }

            foreach (var alias in action.Aliases)
            {
                if (input.Contains(alias, StringComparison.OrdinalIgnoreCase))
                {
                    score = Math.Max(score, 0.85);
                }
            }

            if (score > 0)
            {
                result.Add(new ActionCandidate
                {
                    ActionName = action.ActionName,
                    Score = score,
                    Source = "rule"
                });
            }
        }

        return result.OrderByDescending(x => x.Score).ToArray();
    }
}




