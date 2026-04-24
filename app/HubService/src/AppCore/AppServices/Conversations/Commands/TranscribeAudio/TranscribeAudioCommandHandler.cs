namespace Insurance.HubService.AppCore.AppServices.Conversations.Commands.TranscribeAudio;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.TranscribeAudio;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class TranscribeAudioCommandHandler(IConversationOrchestrator orchestrator)
    : CommandHandler<TranscribeAudioCommand, AudioTranscriptionResult>
{
    private readonly IConversationOrchestrator _orchestrator = orchestrator;

    public override async Task<CommandResult<AudioTranscriptionResult>> Handle(TranscribeAudioCommand command)
    {
        var result = await _orchestrator.TranscribeUserAudioAsync(
            new AudioTranscriptionRequest(command.SessionId, command.UserId, command.AudioBase64, command.Extension, command.MessageId),
            CancellationToken.None);

        return await OkAsync(result);
    }
}





