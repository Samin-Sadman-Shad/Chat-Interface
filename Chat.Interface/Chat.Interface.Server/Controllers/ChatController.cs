using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Chat.Interface.Server.Hubs;

namespace Chat.Interface.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatController(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<ActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        // Simulate processing and streaming response
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", new
        {
            Id = Guid.NewGuid(),
            Content = $"Processing your request: {request.Message}",
            Type = "assistant",
            Timestamp = DateTime.UtcNow
        });

        return Ok();
    }
}

public record SendMessageRequest(string Message, string? SessionId = null);