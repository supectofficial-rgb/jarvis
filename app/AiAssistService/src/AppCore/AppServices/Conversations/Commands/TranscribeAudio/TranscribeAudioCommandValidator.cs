namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Commands.TranscribeAudio;

using FluentValidation;
using Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.TranscribeAudio;

public class TranscribeAudioCommandValidator : AbstractValidator<TranscribeAudioCommand>
{
    public TranscribeAudioCommandValidator()
    {
        RuleFor(x => x.AudioBase64)
            .NotEmpty()
            .WithMessage("audioBase64 is required");
    }
}


