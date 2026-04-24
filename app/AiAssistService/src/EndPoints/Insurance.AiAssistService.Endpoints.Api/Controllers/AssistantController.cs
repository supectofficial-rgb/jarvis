using Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.ProcessTurn;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

namespace Insurance.AiAssistService.Endpoints.Api.Controllers;

[ApiController]
[Route("api/AiAssistService/[controller]")]
[Route("api/assistant")]
public class AssistantController : OysterFxController
{
    [HttpPost("turn")]
    public async Task<IActionResult> Turn([FromBody] ProcessAssistantTurnCommand command)
        => await SendCommand<ProcessAssistantTurnCommand, ProcessAssistantTurnCommandResult>(command);
}


