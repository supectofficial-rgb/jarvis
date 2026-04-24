using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.SendUserMessage;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.StartSession;
using Insurance.HubService.AppCore.Shared.Conversations.Commands.TranscribeAudio;
using Insurance.HubService.AppCore.Shared.Conversations.Queries.GetHistory;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

namespace Insurance.HubService.Endpoints.Api.Controllers;

[ApiController]
[Route("api/HubService/[controller]")]
public class ConversationController : OysterFxController
{
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionCommand command)
        => await SendCommand<StartSessionCommand, ConversationSessionStarted>(command);

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] SendUserMessageCommand command)
        => await SendCommand<SendUserMessageCommand, ConversationReply>(command);

    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe([FromBody] TranscribeAudioCommand command)
        => await SendCommand<TranscribeAudioCommand, AudioTranscriptionResult>(command);

    [HttpGet("{sessionId}/history")]
    public async Task<IActionResult> GetHistory([FromRoute] string sessionId, [FromQuery] int take = 20)
        => await ExecuteQueryAsync<GetConversationHistoryQuery, IReadOnlyList<ConversationMessage>>(
            new GetConversationHistoryQuery
            {
                SessionId = sessionId,
                Take = take
            });
}


