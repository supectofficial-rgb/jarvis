namespace Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.ServiceCallers;

using System.Linq;
using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Constants;
using Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Requests;
using Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models.Responses;
using Microsoft.Extensions.Options;
using OysterFx.AppCore.Shared.DependencyInjections;
using OysterFx.Infra.ServiceCom.ResutfulApi.Caller;

public sealed class AiAssistantService : IAiAssistantClient, ITransientLifetimeMarker
{
    private const string FallbackMessage = "Your message was received. I am preparing the next step.";

    private readonly IOptions<AiAssistantApiOptions> _options;
    private readonly HttpService _httpService;
    private readonly string _baseUrl;

    public AiAssistantService(IOptions<AiAssistantApiOptions> options, HttpService httpService)
    {
        _options = options;
        _httpService = httpService;
        _baseUrl = (_options.Value.BaseUrl ?? string.Empty).TrimEnd('/') + "/";
    }

    public async Task<AiAssistantTurnResult> GetAssistantTurnAsync(AiAssistantRequest request, CancellationToken cancellationToken)
    {
        var options = _options.Value;

        if (!options.Enabled || string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return CreateFallbackTurn(request.SessionId, FallbackMessage);
        }

        var url = BuildUrl(options.ProcessPath, "api/AiAssistService/assistant/turn");
        var payload = new ProcessAssistantTurnRequest
        {
            SessionId = request.SessionId,
            UserId = request.UserId,
            Text = request.Text
        };

        var (success, error) = await _httpService.PostAsync<
            ProcessAssistantTurnRequest,
            CommandResultEnvelope<ProcessAssistantTurnResponse>,
            ErrorResponse>(
            url,
            payload,
            timeoutMs: Math.Max(1000, options.TimeoutMs));

        if (success is not null && success.Data is not null)
        {
            return ToAssistantTurnResult(success.Data, request.SessionId);
        }

        var errorMessage = ResolveAssistantErrorText(success, error);

        return new AiAssistantTurnResult(
            AiAssistantTurnStatus.ExecutionFailed,
            string.IsNullOrWhiteSpace(errorMessage) ? FallbackMessage : errorMessage,
            request.SessionId,
            Guid.NewGuid().ToString("N"),
            null,
            Array.Empty<string>(),
            Array.Empty<string>(),
            null);
    }

    public async Task<AudioTranscriptionResult> TranscribeAudioAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken)
    {
        var options = _options.Value;

        if (!options.Enabled || string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            return new AudioTranscriptionResult(
                request.SessionId ?? string.Empty,
                Guid.NewGuid().ToString("N"),
                request.MessageId ?? string.Empty,
                string.Empty,
                DateTimeOffset.UtcNow);
        }

        var url = BuildUrl(options.TranscribePath, "api/stt/transcribe");
        var payload = new TranscribeAudioRequest
        {
            SessionId = request.SessionId,
            MessageId = request.MessageId,
            AudioBase64 = request.AudioBase64,
            Extension = request.Extension
        };

        var (success, error) = await _httpService.PostAsync<
            TranscribeAudioRequest,
            CommandResultEnvelope<TranscribeAudioResponse>,
            ErrorResponse>(
            url,
            payload,
            timeoutMs: Math.Max(1000, options.TimeoutMs));

        if (success is not null && success.IsSuccess && success.Data is not null)
        {
            return new AudioTranscriptionResult(
                string.IsNullOrWhiteSpace(success.Data.SessionId) ? (request.SessionId ?? string.Empty) : success.Data.SessionId,
                success.Data.CorrelationId,
                success.Data.MessageId,
                success.Data.Text,
                DateTimeOffset.UtcNow);
        }

        var fallbackText = ResolveErrorText(success, error);

        return new AudioTranscriptionResult(
            request.SessionId ?? string.Empty,
            Guid.NewGuid().ToString("N"),
            request.MessageId ?? string.Empty,
            fallbackText,
            DateTimeOffset.UtcNow);
    }

    private string BuildUrl(string path, string defaultPath)
    {
        var normalizedPath = string.IsNullOrWhiteSpace(path)
            ? defaultPath
            : path.TrimStart('/');

        return _baseUrl + normalizedPath;
    }

    private static AiAssistantTurnResult CreateFallbackTurn(string sessionId, string message)
        => new(
            AiAssistantTurnStatus.MessageOnly,
            message,
            sessionId,
            Guid.NewGuid().ToString("N"),
            null,
            Array.Empty<string>(),
            Array.Empty<string>(),
            null);

    private static AiAssistantTurnResult ToAssistantTurnResult(ProcessAssistantTurnResponse response, string fallbackSessionId)
    {
        var normalizedMissingFields = (response.MissingFields ?? new List<string>())
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Select(static x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var normalizedSuggestions = (response.Suggestions ?? new List<string>())
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Select(static x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var status = string.IsNullOrWhiteSpace(response.Status)
            ? AiAssistantTurnStatus.MessageOnly
            : response.Status.Trim();

        var correlationId = string.IsNullOrWhiteSpace(response.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : response.CorrelationId.Trim();

        var sessionId = string.IsNullOrWhiteSpace(response.SessionId)
            ? fallbackSessionId
            : response.SessionId.Trim();

        return new AiAssistantTurnResult(
            status,
            response.Message?.Trim() ?? string.Empty,
            sessionId,
            correlationId,
            string.IsNullOrWhiteSpace(response.ActionName) ? null : response.ActionName.Trim(),
            normalizedMissingFields,
            normalizedSuggestions,
            response.Data);
    }

    private static string ResolveAssistantErrorText(CommandResultEnvelope<ProcessAssistantTurnResponse>? success, ErrorResponse? error)
    {
        if (success is not null && success.ErrorMessages.Count > 0)
        {
            return success.ErrorMessages[0];
        }

        if (error is not null && error.Errors.Count > 0)
        {
            return error.Errors[0];
        }

        return string.Empty;
    }

    private static string ResolveErrorText(CommandResultEnvelope<TranscribeAudioResponse>? success, ErrorResponse? error)
    {
        if (success is not null && success.ErrorMessages.Count > 0)
        {
            return success.ErrorMessages[0];
        }

        if (error is not null && error.Errors.Count > 0)
        {
            return error.Errors[0];
        }

        return string.Empty;
    }
}


