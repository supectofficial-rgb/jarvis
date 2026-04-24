using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public sealed class VectorRetrievalService : IVectorRetrievalService, ISingletoneLifetimeMarker
{
    public Task<IReadOnlyList<ActionCandidate>> ResolveAsync(string normalizedText, IReadOnlyList<ActionMetadata> catalog, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<ActionCandidate>>(Array.Empty<ActionCandidate>());
    }
}




