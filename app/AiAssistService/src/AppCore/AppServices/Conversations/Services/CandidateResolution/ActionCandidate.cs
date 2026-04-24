namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;

public sealed class ActionCandidate
{
    public string ActionName { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Source { get; set; } = string.Empty;
}


