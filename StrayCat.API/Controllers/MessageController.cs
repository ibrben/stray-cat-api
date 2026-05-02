using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers;

[ApiController]
[Route("api/msg")]
public class MessageController : ControllerBase
{
    private readonly ILineMessagingService _lineMessagingService;

    public MessageController(ILineMessagingService lineMessagingService)
    {
        _lineMessagingService = lineMessagingService;
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download()
    {
        var success = await _lineMessagingService.SendDownloadNotificationAsync();
        
        if (success)
        {
            return Ok(new { message = "Download notification sent successfully" });
        }
        else
        {
            return StatusCode(500, new { message = "Failed to send download notification" });
        }
    }
}
