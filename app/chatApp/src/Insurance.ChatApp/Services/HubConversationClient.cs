namespace Insurance.ChatApp.Services;

using System.Net.Http.Json;
using System.Text.Json;
using Insurance.ChatApp.Configuration;
using Insurance.ChatApp.Models.Api;
using Insurance.ChatApp.Models.Chat;
using Microsoft.Extensions.Options;

public sealed class HubConversationClient : IHubConversationClient
{
    private readonly HttpClient _httpClient;
    private readonly HubServiceOptions _options;
    private readonly ILogger<HubConversationClient> _logger;

    public HubConversationClient(HttpClient httpClient, IOptions<HubServiceOptions> options, ILogger<HubConversationClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<AppApiResponse<ConversationSessionStartedDto>> StartSessionAsync(StartSessionRequest request, CancellationToken cancellationToken)
        => PostCommandAsync<StartSessionRequest, ConversationSessionStartedDto>("start", request, cancellationToken);

    public Task<AppApiResponse<ConversationReplyDto>> SendMessageAsync(SendUserMessageRequest request, CancellationToken cancellationToken)
        => PostCommandAsync<SendUserMessageRequest, ConversationReplyDto>("message", request, cancellationToken);

    public Task<AppApiResponse<AudioTranscriptionResultDto>> TranscribeAsync(TranscribeAudioRequest request, CancellationToken cancellationToken)
        => PostCommandAsync<TranscribeAudioRequest, AudioTranscriptionResultDto>("transcribe", request, cancellationToken);

    public async Task<AppApiResponse<IReadOnlyList<ConversationMessageDto>>> GetHistoryAsync(string sessionId, int take, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Failure("Session id is required.");
        }

        var endpoint = BuildEndpoint($"{Uri.EscapeDataString(sessionId.Trim())}/history?take={Math.Max(1, take)}");

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Hub history request failed ({StatusCode}): {Body}", (int)response.StatusCode, content);
                return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Failure($"History request failed ({(int)response.StatusCode}).");
            }

            var envelope = JsonSerializer.Deserialize<QueryResultEnvelope<IReadOnlyList<ConversationMessageDto>>>(content, JsonOptions);
            if (envelope is null)
            {
                return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Failure("History response is empty.");
            }

            if (!envelope.IsSuccess)
            {
                return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Failure(
                    envelope.ErrorMessage,
                    envelope.ValidationErrors);
            }

            return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Success(envelope.Data ?? Array.Empty<ConversationMessageDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching conversation history.");
            return AppApiResponse<IReadOnlyList<ConversationMessageDto>>.Failure("Could not fetch conversation history.");
        }
    }

    private async Task<AppApiResponse<TResponse>> PostCommandAsync<TRequest, TResponse>(string action, TRequest request, CancellationToken cancellationToken)
    {
        var endpoint = BuildEndpoint(action);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Hub command request failed ({StatusCode}): {Body}", (int)response.StatusCode, content);
            }

            var envelope = JsonSerializer.Deserialize<CommandResultEnvelope<TResponse>>(content, JsonOptions);
            if (envelope is null)
            {
                return AppApiResponse<TResponse>.Failure("Response payload is empty.");
            }

            if (!envelope.IsSuccess)
            {
                return AppApiResponse<TResponse>.Failure(
                    envelope.ErrorMessages.FirstOrDefault(),
                    envelope.ErrorMessages);
            }

            return AppApiResponse<TResponse>.Success(envelope.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while posting action '{Action}' to HubService.", action);
            return AppApiResponse<TResponse>.Failure("HubService is currently unavailable.");
        }
    }

    private string BuildEndpoint(string action)
    {
        var basePath = string.IsNullOrWhiteSpace(_options.ConversationBasePath)
            ? "api/HubService/Conversation"
            : _options.ConversationBasePath.Trim('/');

        var normalizedAction = action.TrimStart('/');
        return $"{basePath}/{normalizedAction}";
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class CommandResultEnvelope<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
    }

    private sealed class QueryResultEnvelope<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public IEnumerable<string>? ValidationErrors { get; set; }
    }
}
