using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;

public sealed class IntentLlmRequest
{
    public string UserText { get; set; } = string.Empty;
    public string PromptText { get; set; } = string.Empty;
    public IReadOnlyList<ActionCandidate> Candidates { get; set; } = Array.Empty<ActionCandidate>();
    public IReadOnlyList<ActionMetadata> CatalogSlice { get; set; } = Array.Empty<ActionMetadata>();
}


