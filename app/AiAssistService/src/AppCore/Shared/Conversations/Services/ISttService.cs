namespace Insurance.AiAssistService.AppCore.Shared.Conversations.Services;

public interface ISttService
{
    Task<string> TranscribeAsync(string filePath);
}
