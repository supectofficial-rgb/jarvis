using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;
using System.Text.Json;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

public sealed class ResultEventHandler : IResultEventHandler, ISingletoneLifetimeMarker
{
    private readonly ISessionService _sessionService;
    private readonly IDialogueStateManager _dialogueStateManager;
    private readonly IResponseComposer _responseComposer;
    private readonly IMessagePublisher _publisher;
    private readonly IAuditTrailService _auditTrailService;

    public ResultEventHandler(
        ISessionService sessionService,
        IDialogueStateManager dialogueStateManager,
        IResponseComposer responseComposer,
        IMessagePublisher publisher,
        IAuditTrailService auditTrailService)
    {
        _sessionService = sessionService;
        _dialogueStateManager = dialogueStateManager;
        _responseComposer = responseComposer;
        _publisher = publisher;
        _auditTrailService = auditTrailService;
    }

    public async Task HandleAsync(string eventName, string payload, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsCompletionEvent(eventName))
        {
            return;
        }

        var message = Deserialize(payload);
        if (message is null || string.IsNullOrWhiteSpace(message.SessionId))
        {
            return;
        }

        var session = await _sessionService.TryGetAsync(message.SessionId, cancellationToken);
        if (session is null)
        {
            return;
        }

        var correlationId = string.IsNullOrWhiteSpace(message.CorrelationId)
            ? session.CorrelationId ?? Guid.NewGuid().ToString("N")
            : message.CorrelationId;

        AssistantTurnResponse response;
        if (message.Success)
        {
            session.CurrentState = _dialogueStateManager.MoveToCompleted();
            session.ClearPendingAction();
            response = _responseComposer.ComposeExecutionCompleted(session.SessionId, correlationId, message.ActionName, message.Data);
        }
        else
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            response = _responseComposer.ComposeExecutionFailed(
                session.SessionId,
                correlationId,
                message.ActionName,
                string.IsNullOrWhiteSpace(message.Reason) ? "Execution failed." : message.Reason);
        }

        session.CorrelationId = correlationId;
        session.LastTurnSummary = response.Message;
        session.LastResponseStatus = response.Status;

        await _sessionService.SaveAsync(session, cancellationToken);

        await _publisher.PublishAsync("assistant.turn.response", response, cancellationToken);

        await _auditTrailService.RecordAsync(new TurnAuditRecord
        {
            SessionId = session.SessionId,
            CorrelationId = correlationId,
            Step = "async_result_handled",
            Payload = JsonSerializer.Serialize(new { eventName, response })
        }, cancellationToken);
    }

    private static bool IsCompletionEvent(string eventName)
        => eventName.Equals("assistant.action.completed", StringComparison.OrdinalIgnoreCase)
            || eventName.Equals("assistant.action.failed", StringComparison.OrdinalIgnoreCase);

    private static AsyncExecutionResultEvent? Deserialize(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AsyncExecutionResultEvent>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }
}




