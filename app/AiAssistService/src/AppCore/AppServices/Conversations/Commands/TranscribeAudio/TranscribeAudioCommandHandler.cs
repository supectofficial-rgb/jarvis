namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Commands.TranscribeAudio;

using Insurance.AiAssistService.AppCore.Shared.Conversations.Services;
using Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.TranscribeAudio;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class TranscribeAudioCommandHandler : CommandHandler<TranscribeAudioCommand, TranscribeAudioCommandResult>
{
    private readonly ISttService _sttService;

    public TranscribeAudioCommandHandler(ISttService sttService)
    {
        _sttService = sttService;
    }

    public override async Task<CommandResult<TranscribeAudioCommandResult>> Handle(TranscribeAudioCommand command)
    {
        if (!TryDecodeBase64(command.AudioBase64, out var audioBytes))
        {
            return CommandResult<TranscribeAudioCommandResult>.Failure("Invalid audioBase64 payload.");
        }

        var correlationId = Guid.NewGuid().ToString("N");
        var tempFilePath = BuildTempPath(command.Extension);

        try
        {
            await File.WriteAllBytesAsync(tempFilePath, audioBytes);
            var text = await _sttService.TranscribeAsync(tempFilePath);

            var result = new TranscribeAudioCommandResult
            {
                SessionId = command.SessionId ?? string.Empty,
                MessageId = command.MessageId ?? string.Empty,
                CorrelationId = correlationId,
                Text = text?.Trim() ?? string.Empty
            };

            return CommandResult<TranscribeAudioCommandResult>.Success(result);
        }
        catch (Exception ex)
        {
            return CommandResult<TranscribeAudioCommandResult>.Failure($"Speech-to-text failed: {ex.Message}");
        }
        finally
        {
            TryDelete(tempFilePath);
        }
    }

    private static bool TryDecodeBase64(string input, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var payload = input.Trim();
        var commaIndex = payload.IndexOf(',');

        if (payload.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex > -1)
        {
            payload = payload[(commaIndex + 1)..];
        }

        try
        {
            bytes = Convert.FromBase64String(payload);
            return bytes.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildTempPath(string? extension)
    {
        var normalizedExtension = NormalizeExtension(extension);
        var fileName = $"aiassist-stt-{Guid.NewGuid():N}{normalizedExtension}";
        return Path.Combine(Path.GetTempPath(), fileName);
    }

    private static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return ".wav";
        }

        var ext = extension.Trim();
        if (!ext.StartsWith('.'))
        {
            ext = "." + ext;
        }

        return ext.Length > 10 ? ".wav" : ext.ToLowerInvariant();
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best effort cleanup.
        }
    }
}


