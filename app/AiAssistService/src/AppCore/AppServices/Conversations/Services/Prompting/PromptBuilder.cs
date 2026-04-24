using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;
using System.Text;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Prompting;

public sealed class PromptBuilder : IPromptBuilder, ISingletoneLifetimeMarker
{
    public LlmPrompt Build(string normalizedText, AssistantSession session, IReadOnlyList<ActionCandidate> candidates, IReadOnlyList<ActionMetadata> catalog)
    {
        var topCandidates = candidates.Take(3).ToArray();
        var candidateNames = topCandidates.Select(x => x.ActionName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var catalogSlice = catalog.Where(x => candidateNames.Contains(x.ActionName)).ToArray();

        var sb = new StringBuilder();
        sb.AppendLine("Extract user intent and parameters as JSON.");
        sb.AppendLine("Use only actions below:");

        foreach (var action in catalogSlice)
        {
            sb.Append("- ")
              .Append(action.ActionName)
              .Append(" required: ")
              .Append(string.Join(',', action.RequiredParams))
              .AppendLine();
        }

        sb.Append("User text: ").AppendLine(normalizedText);

        return new LlmPrompt
        {
            PromptText = sb.ToString(),
            UserText = normalizedText,
            Candidates = topCandidates,
            CatalogSlice = catalogSlice
        };
    }
}




