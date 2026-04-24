namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;

public interface IAuditTrailService
{
    Task RecordAsync(TurnAuditRecord record, CancellationToken cancellationToken);
}


