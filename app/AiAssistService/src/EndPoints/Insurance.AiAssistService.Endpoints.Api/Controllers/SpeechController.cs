using Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.TranscribeAudio;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

namespace Insurance.AiAssistService.Endpoints.Api.Controllers;

[ApiController]
[Route("api/AiAssistService/[controller]")]
[Route("api/stt")]
public class SpeechController : OysterFxController
{
    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe([FromBody] TranscribeAudioCommand command)
        => await SendCommand<TranscribeAudioCommand, TranscribeAudioCommandResult>(command);
}


