using Insurance.HubService.AppCore.Shared.Conversations.Options;
using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OysterFx.AppCore.Shared.DependencyInjections;

namespace Insurance.HubService.AppCore.AppServices.Conversations.Services;

public sealed class ConversationOrchestrator : IConversationOrchestrator, IScopeLifetimeMarker
{
    private readonly IConversationSessionStore _sessionStore;
    private readonly IAiAssistantClient _aiAssistantClient;
    private readonly IConversationAssistantMessageResolverDomainService _assistantMessageResolver;
    private readonly HubConversationOptions _options;
    private readonly ILogger<ConversationOrchestrator> _logger;

    public ConversationOrchestrator(
        IConversationSessionStore sessionStore,
        IAiAssistantClient aiAssistantClient,
        IConversationAssistantMessageResolverDomainService assistantMessageResolver,
        IOptions<HubConversationOptions> options,
        ILogger<ConversationOrchestrator> logger)
    {
        _sessionStore = sessionStore;
        _aiAssistantClient = aiAssistantClient;
        _assistantMessageResolver = assistantMessageResolver;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ConversationSessionStarted> StartSessionAsync(string? sessionId, string? userId, CancellationToken cancellationToken)
    {
        var session = await _sessionStore.GetOrCreateAsync(sessionId, userId, cancellationToken);
        return new ConversationSessionStarted(session.SessionId, session.UserId, session.CreatedAtUtc);
    }

    public async Task<ConversationReply> HandleUserMessageAsync(UserMessageRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            throw new ArgumentException("User message cannot be empty.", nameof(request));
        }

        var session = await _sessionStore.GetOrCreateAsync(request.SessionId, request.UserId, cancellationToken);

        session.AppendMessage(ConversationMessageRole.User, request.Text);
        await _sessionStore.SaveAsync(session, cancellationToken);

        var assistantTurn = await _aiAssistantClient.GetAssistantTurnAsync(
            new AiAssistantRequest(session.SessionId, session.UserId, request.Text.Trim()),
            cancellationToken);

        var assistantMessage = _assistantMessageResolver.ResolveDisplayMessage(assistantTurn);

        session.AppendMessage(ConversationMessageRole.Assistant, assistantMessage);
        await _sessionStore.SaveAsync(session, cancellationToken);

        var updatedHistory = session.GetMessages(Math.Max(1, _options.MaxHistoryMessages));

        _logger.LogInformation(
            "Conversation handled for session {SessionId} with status {Status} and correlation {CorrelationId}.",
            session.SessionId,
            assistantTurn.Status,
            assistantTurn.CorrelationId);

        return new ConversationReply(
            session.SessionId,
            assistantMessage,
            assistantTurn,
            updatedHistory,
            DateTimeOffset.UtcNow);
    }

    public async Task<AudioTranscriptionResult> TranscribeUserAudioAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AudioBase64))
        {
            throw new ArgumentException("Audio payload cannot be empty.", nameof(request));
        }

        var session = await _sessionStore.GetOrCreateAsync(request.SessionId, request.UserId, cancellationToken);

        var transcription = await _aiAssistantClient.TranscribeAudioAsync(
            request with
            {
                SessionId = session.SessionId,
                UserId = session.UserId
            },
            cancellationToken);

        _logger.LogInformation("Audio transcribed for session {SessionId}.", session.SessionId);

        return transcription;
    }

    public async Task<IReadOnlyList<ConversationMessage>> GetHistoryAsync(string sessionId, int maxItems, CancellationToken cancellationToken)
    {
        var session = await _sessionStore.GetAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return Array.Empty<ConversationMessage>();
        }

        return session.GetMessages(Math.Max(1, maxItems));
    }
}


