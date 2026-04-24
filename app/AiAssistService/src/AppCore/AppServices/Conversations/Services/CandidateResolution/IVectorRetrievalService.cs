using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public interface IVectorRetrievalService
{
    Task<IReadOnlyList<ActionCandidate>> ResolveAsync(string normalizedText, IReadOnlyList<ActionMetadata> catalog, CancellationToken cancellationToken);
}


