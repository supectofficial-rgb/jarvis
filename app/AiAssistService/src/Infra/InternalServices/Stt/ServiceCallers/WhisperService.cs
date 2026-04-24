using Insurance.AiAssistService.AppCore.Shared.Conversations.Services;
using System.Diagnostics;

namespace Insurance.AiAssistService.Infra.InternalServices.STT;

public class WhisperService : ISttService
{
    public async Task<string> TranscribeAsync(string filePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "whisper",
                Arguments = $"\"{filePath}\" --language Persian",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        string result = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        return result.Trim();
    }
}

