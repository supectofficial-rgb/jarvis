using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public interface IRuleBasedCandidateProvider
{
    IReadOnlyList<ActionCandidate> Resolve(string normalizedText, IReadOnlyList<ActionMetadata> catalog);
}


