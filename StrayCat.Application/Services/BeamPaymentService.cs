using StrayCat.Application.Interfaces;
using StrayCat.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using StrayCat.Application.Settings;

namespace StrayCat.Application.Services;
public class BeamPaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public BeamPaymentService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }
    public Task<string> CreatePaymentIntentAsync(decimal amount, string currency = "thb")
    {
        throw new NotImplementedException();
    }

    public Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode)
    {
        throw new NotImplementedException();
    }
    
    public async Task<string> CreatePaymentUrl(string productName, int productId, string refCode, decimal amount, decimal unitPrice, int quantity, string imgUrl)
    {
        var redirectUrl = _configuration["Frontend:Url"];

        PaymentSettings paymentSettings = new PaymentSettings();
        _configuration.GetSection("Payment").Bind(paymentSettings);
        if (string.IsNullOrEmpty(paymentSettings.Url) || string.IsNullOrEmpty(paymentSettings.ApiKey) || string.IsNullOrEmpty(paymentSettings.ApiId))
        {
            throw new ArgumentNullException("Beam configuration is missing required values");
        }

        var request = new BeamCheckoutDto
        {
            CollectDeliveryAddress = false,
            CollectPhoneNumber = false,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            FeeType = "TRANSACTION_FEE",
            LinkSettings = new LinkSettingsDto
            {
                BuyNowPayLater = new PaymentMethodDto { IsEnabled = false },
                Card = new PaymentMethodDto { IsEnabled = paymentSettings.AllowedMethods.Card },
                CardInstallments = new CardInstallmentsDto
                {
                    IsEnabled = false,
                    Installments3m = new InstallmentOptionDto { IsEnabled = false },
                    Installments4m = new InstallmentOptionDto { IsEnabled = false },
                    Installments6m = new InstallmentOptionDto { IsEnabled = false },
                    Installments10m = new InstallmentOptionDto { IsEnabled = false }
                },
                EWallets = new PaymentMethodDto { IsEnabled = paymentSettings.AllowedMethods.EWallets },
                MobileBanking = new PaymentMethodDto { IsEnabled = paymentSettings.AllowedMethods.MobileBanking },
                QrPromptPay = new PaymentMethodDto { IsEnabled = paymentSettings.AllowedMethods.QrPromptPay }
            },
            Order = new OrderDto
            {
                Currency = "THB",
                Description = productName,
                InternalNote = $"{productName}:{refCode}",
                NetAmount = amount * 100,
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        Description = productName,
                        ImageUrl = imgUrl,
                        ItemName = productName,
                        Price = unitPrice * 100,
                        ProductId = refCode,
                        Quantity = quantity,
                        Sku = refCode
                    }
                },
                ReferenceId = refCode
            },
            RedirectUrl = $"{redirectUrl}/exp/{productId}/confirm?ref={refCode}&title={productName}"
        };

        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{paymentSettings.ApiId}:{paymentSettings.ApiKey}"));
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{paymentSettings.Url}/api/v1/payment-links");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credential);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        var result = await _httpClient.SendAsync(requestMessage);
        
        if (!result.IsSuccessStatusCode)
        {
            var errorResponse = await result.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Beam API request failed with status {result.StatusCode}: {errorResponse}");
        }

        var response = await result.Content.ReadAsStringAsync();
        var beamResult = JsonSerializer.Deserialize<BeamCheckoutResponseDto>(response);

        if (beamResult == null || string.IsNullOrEmpty(beamResult.Url))
        {
            throw new InvalidOperationException("Failed to deserialize Beam response or URL is empty");
        }

        return beamResult.Url;
    }
}
