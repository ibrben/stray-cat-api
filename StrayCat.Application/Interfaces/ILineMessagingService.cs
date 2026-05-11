namespace StrayCat.Application.Interfaces;

public interface ILineMessagingService
{
    Task<bool> SendDownloadNotificationAsync(string msg);
    Task<bool> ProcessWebhookAsync(LineWebhookRequest webhookRequest);
}
