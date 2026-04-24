namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public interface ICandidateResolver
{
    IReadOnlyList<ActionCandidate> Resolve(IReadOnlyList<ActionCandidate> ruleCandidates, IReadOnlyList<ActionCandidate> vectorCandidates, int take);
}


