using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.SendUserMessage;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.StartSession;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.TranscribeAudio;
using OysterFx.AppCore.Shared.Commands;
using Microsoft.AspNetCore.SignalR;

namespace Insurance.HubService.Endpoints.Api.Hubs;

public class LiveAssistHub : Hub
{
    private readonly ICommandBus _commandBus;
    private readonly ILogger<LiveAssistHub> _logger;

    public LiveAssistHub(ICommandBus commandBus, ILogger<LiveAssistHub> logger)
    {
        _commandBus = commandBus;
        _logger = logger;
    }

    public async Task<ConversationSessionStarted> StartSession(string? sessionId, string? userId, CancellationToken cancellationToken = default)
    {
        var commandResult = await _commandBus.SendAsync<StartSessionCommand, ConversationSessionStarted>(
            new StartSessionCommand
            {
                SessionId = sessionId,
                UserId = userId
            });

        if (!commandResult.IsSuccess)
        {
            throw new HubException(commandResult.ErrorMessages.FirstOrDefault() ?? "Unable to start session.");
        }

        var started = commandResult.Data;

        await Groups.AddToGroupAsync(Context.ConnectionId, started.SessionId, cancellationToken);
        await Clients.Caller.SendAsync("session_started", started, cancellationToken);

        return started;
    }

    public async Task<AudioTranscriptionResult> TranscribeAudio(string sessionId, string audioBase64, string? extension = null, string? messageId = null, string? userId = null, CancellationToken cancellationToken = default)
    {
        var commandResult = await _commandBus.SendAsync<TranscribeAudioCommand, AudioTranscriptionResult>(
            new TranscribeAudioCommand
            {
                SessionId = sessionId,
                UserId = userId,
                AudioBase64 = audioBase64,
                Extension = extension,
                MessageId = messageId
            });

        if (!commandResult.IsSuccess)
        {
            throw new HubException(commandResult.ErrorMessages.FirstOrDefault() ?? "Unable to transcribe audio.");
        }

        var result = commandResult.Data;

        await Clients.Caller.SendAsync("stt_text", result, cancellationToken);

        return result;
    }

    public async Task<ConversationReply> SendUserMessage(string sessionId, string text, string? userId = null, CancellationToken cancellationToken = default)
    {
        var commandResult = await _commandBus.SendAsync<SendUserMessageCommand, ConversationReply>(
            new SendUserMessageCommand
            {
                SessionId = sessionId,
                UserId = userId,
                Text = text
            });

        if (!commandResult.IsSuccess)
        {
            throw new HubException(commandResult.ErrorMessages.FirstOrDefault() ?? "Unable to process message.");
        }

        var reply = commandResult.Data;

        await Clients.Group(reply.SessionId).SendAsync("assistant_message", reply, cancellationToken);

        return reply;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Hub client connected: {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Hub client disconnected: {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}


