namespace StrayCat.Application.Interfaces;

public interface ILineMessagingService
{
    Task<bool> SendDownloadNotificationAsync();
}
