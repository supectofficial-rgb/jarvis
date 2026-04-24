namespace Insurance.ChatApp.Controllers;

using Insurance.ChatApp.Models.Chat;
using Insurance.ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("chat/api")]
public sealed class ChatApiController : ControllerBase
{
    private readonly IHubConversationClient _hubConversationClient;

    public ChatApiController(IHubConversationClient hubConversationClient)
    {
        _hubConversationClient = hubConversationClient;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartSessionRequest request, CancellationToken cancellationToken)
    {
        var response = await _hubConversationClient.StartSessionAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromBody] SendUserMessageRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            return BadRequest(new { isSuccess = false, errorMessage = "SessionId is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { isSuccess = false, errorMessage = "Text is required." });
        }

        var response = await _hubConversationClient.SendMessageAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe([FromBody] TranscribeAudioRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SessionId))
        {
            return BadRequest(new { isSuccess = false, errorMessage = "SessionId is required." });
        }

        if (string.IsNullOrWhiteSpace(request.AudioBase64))
        {
            return BadRequest(new { isSuccess = false, errorMessage = "AudioBase64 is required." });
        }

        var response = await _hubConversationClient.TranscribeAsync(request, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] string sessionId, [FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var response = await _hubConversationClient.GetHistoryAsync(sessionId, take, cancellationToken);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
