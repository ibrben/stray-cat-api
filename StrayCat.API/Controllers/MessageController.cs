using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.Interfaces;
using StrayCat.Application.DTOs;

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
        var success = await _lineMessagingService.SendDownloadNotificationAsync("Download notification sent successfully");
        
        if (success)
        {
            return Ok(new { message = "Download notification sent successfully" });
        }
        else
        {
            return StatusCode(500, new { message = "Failed to send download notification" });
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] LineWebhookRequest webhookRequest)
    {
        try
        {
            if (webhookRequest == null)
            {
                return BadRequest(new { message = "Invalid webhook request" });
            }

            var success = await _lineMessagingService.ProcessWebhookAsync(webhookRequest);
            
            if (success)
            {
                return Ok(new { message = "Webhook processed successfully" });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to process webhook" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing webhook: {ex.Message}");
            return StatusCode(500, new { message = "Failed to process webhook" });
        }
        
        return Ok(new { message = "Webhook processing completed" });
    }
}
