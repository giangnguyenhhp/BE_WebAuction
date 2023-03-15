using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web_Auction.Hubs;

namespace Web_Auction.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        await _hubContext.Clients.All.SendAsync("messageReceived", message);
        return Ok();
    }
}