using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StrayCat.Application.Interfaces;
using StrayCat.Application.DTOs;

namespace StrayCat.Application.Services;

public class LineMessagingService : ILineMessagingService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _channelAccessToken;
    private readonly string _toUserId;
    private readonly string _toGroupId;

    public LineMessagingService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _channelAccessToken = _configuration["LineMessaging:ChannelAccessToken"] ?? throw new ArgumentNullException("LineMessaging:ChannelAccessToken");
        _toUserId = _configuration["LineMessaging:ToUserId"] ?? throw new ArgumentNullException("LineMessaging:ToUserId");
        _toGroupId = _configuration["LineMessaging:ToGroupId"] ?? throw new ArgumentNullException("LineMessaging:ToGroupId");
    }

    public async Task<bool> SendDownloadNotificationAsync(string msg, string? destination)
    {  
        msg = msg?? "There is download occur on your page.";
        string to = destination?? _toUserId;
        try
        {
            var messagePayload = new
            {
                to = to,
                messages = new[]
                {
                    new
                    {
                        type = "text",
                        text = msg
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

    public async Task<bool> ProcessWebhookAsync(LineWebhookRequest webhookRequest)
    {
        try
        {
            if (webhookRequest?.Events == null || !webhookRequest.Events.Any())
            {
                Console.WriteLine("No events found in webhook");
                return false;
            }

            foreach (var webhookEvent in webhookRequest.Events)
            {
                string msg = $"Destination: {webhookRequest.Destination}, Msg: {webhookEvent.Message?.Text}";
                await SendDownloadNotificationAsync(msg, _toGroupId);
                await ProcessWebhookEvent(webhookEvent);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing webhook: {ex.Message}");
            return false;
        }
    }

    private async Task ProcessWebhookEvent(LineWebhookEvent webhookEvent)
    {
        try
        {
            switch (webhookEvent.Type.ToLower())
            {
                case "message":
                    await HandleMessageEvent(webhookEvent);
                    break;
                case "follow":
                    await HandleFollowEvent(webhookEvent);
                    break;
                case "unfollow":
                    await HandleUnfollowEvent(webhookEvent);
                    break;
                default:
                    Console.WriteLine($"Unhandled webhook event type: {webhookEvent.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing webhook event: {ex.Message}");
        }
    }

    private async Task HandleMessageEvent(LineWebhookEvent webhookEvent)
    {
        if (webhookEvent.Message?.Type.ToLower() == "text" && !string.IsNullOrEmpty(webhookEvent.Message.Text))
        {
            var messageText = webhookEvent.Message.Text.ToLower();
            var responseText = GetResponseForMessage(messageText);

            if (!string.IsNullOrEmpty(responseText))
            {
                await SendMessageAsync(webhookEvent.Source.UserId, responseText);
            }
        }
    }

    private async Task HandleFollowEvent(LineWebhookEvent webhookEvent)
    {
        var welcomeMessage = "Welcome! Thank you for following us. How can we help you today?";
        await SendMessageAsync(webhookEvent.Source.UserId, welcomeMessage);
    }

    private async Task HandleUnfollowEvent(LineWebhookEvent webhookEvent)
    {
        // Handle user unfollow (cleanup user data if needed)
        Console.WriteLine($"User {webhookEvent.Source.UserId} unfollowed");
    }

    private async Task<bool> SendMessageAsync(string toUserId, string message)
    {
        try
        {
            var messagePayload = new
            {
                to = toUserId,
                messages = new[]
                {
                    new
                    {
                        type = "text",
                        text = message
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
            Console.WriteLine($"Error sending LINE message to user {toUserId}: {ex.Message}");
            return false;
        }
    }

    private string GetResponseForMessage(string messageText)
    {
        return messageText switch
        {
            var msg when msg.Contains("hello") || msg.Contains("hi") => "Hello! How can I assist you today?",
            var msg when msg.Contains("booking") => "I can help you with booking information. Please provide your booking reference code.",
            var msg when msg.Contains("help") => "Available commands:\n- 'booking' - Get booking information\n- 'help' - Show this help message",
            _ => "I'm not sure how to respond to that. Type 'help' for available commands."
        };
    }
}
