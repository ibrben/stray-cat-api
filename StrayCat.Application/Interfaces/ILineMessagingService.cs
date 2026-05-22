using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces;

public interface ILineMessagingService
{
    Task<bool> SendDownloadNotificationAsync(string msg, string? destination = null);
    Task<bool> SendMessageAsync(string toUserId, string message);
    Task<bool> ProcessWebhookAsync(LineWebhookRequest webhookRequest);
}
