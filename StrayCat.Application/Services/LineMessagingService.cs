using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StrayCat.Application.Interfaces;

namespace StrayCat.Application.Services;

public class LineMessagingService : ILineMessagingService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _channelAccessToken;
    private readonly string _toUserId;

    public LineMessagingService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _channelAccessToken = _configuration["LineMessaging:ChannelAccessToken"] ?? throw new ArgumentNullException("LineMessaging:ChannelAccessToken");
        _toUserId = _configuration["LineMessaging:ToUserId"] ?? throw new ArgumentNullException("LineMessaging:ToUserId");
    }

    public async Task<bool> SendDownloadNotificationAsync()
    {
        try
        {
            var messagePayload = new
            {
                to = _toUserId,
                messages = new[]
                {
                    new
                    {
                        type = "text",
                        text = "There is download occur on your page."
                    }
                }
            };

            var json = JsonSerializer.Serialize(messagePayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_channelAccessToken}");
            _httpClient.DefaultRequestHeaders.Add("X-Line-Retry-Key", Guid.NewGuid().ToString());

            var response = await _httpClient.PostAsync("https://api.line.me/v2/bot/message/push", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // Log exception here if needed
            Console.WriteLine($"Error sending LINE message: {ex.Message}");
            return false;
        }
    }
}
