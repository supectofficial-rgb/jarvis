namespace Insurance.ChatApp.Services;

using Insurance.ChatApp.Models.Api;
using Insurance.ChatApp.Models.Chat;

public interface IHubConversationClient
{
    Task<AppApiResponse<ConversationSessionStartedDto>> StartSessionAsync(StartSessionRequest request, CancellationToken cancellationToken);
    Task<AppApiResponse<ConversationReplyDto>> SendMessageAsync(SendUserMessageRequest request, CancellationToken cancellationToken);
    Task<AppApiResponse<AudioTranscriptionResultDto>> TranscribeAsync(TranscribeAudioRequest request, CancellationToken cancellationToken);
    Task<AppApiResponse<IReadOnlyList<ConversationMessageDto>>> GetHistoryAsync(string sessionId, int take, CancellationToken cancellationToken);
}
