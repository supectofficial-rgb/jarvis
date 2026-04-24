namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;

public sealed class TurnAuditRecord
{
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}


