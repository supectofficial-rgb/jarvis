using OysterFx.AppCore.Shared.DependencyInjections;
namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public sealed class CandidateResolver : ICandidateResolver, ISingletoneLifetimeMarker
{
    public IReadOnlyList<ActionCandidate> Resolve(IReadOnlyList<ActionCandidate> ruleCandidates, IReadOnlyList<ActionCandidate> vectorCandidates, int take)
    {
        var merged = ruleCandidates
            .Concat(vectorCandidates)
            .GroupBy(x => x.ActionName, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ActionCandidate
            {
                ActionName = group.Key,
                Score = group.Max(x => x.Score),
                Source = string.Join('+', group.Select(x => x.Source).Distinct(StringComparer.OrdinalIgnoreCase))
            })
            .OrderByDescending(x => x.Score)
            .Take(Math.Max(1, take))
            .ToArray();

        return merged;
    }
}




