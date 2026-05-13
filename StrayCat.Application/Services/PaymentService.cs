using Microsoft.Extensions.Configuration;
using StrayCat.Application.Interfaces;
using Stripe;

namespace StrayCat.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency = "usd")
    {
        try
        {
            var paymentMethod = _configuration.GetSection("Payment:Method")
                .GetChildren()
                .Select(c => c.Value)
                .ToList();

            if (paymentMethod.Count == 0)
            {
                throw new InvalidOperationException("Payment method not configured properly");
            }
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount*100), // Stripe expects amount in cents
                Currency = currency,
                PaymentMethodTypes = paymentMethod,
                // AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                // {
                //     Enabled = false,
                // },
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.ClientSecret;
        }
        catch (Exception ex)
        {
            // Log exception here if needed
            Console.WriteLine($"Error creating payment intent: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode)
    {
        try
        {
            // Find booking by bookingId and referenceCode
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(confirmationId);
            
            if (paymentIntent == null)
            {
                Console.WriteLine($"Payment intent not found for confirmation ID: {confirmationId}");
                return false;
            }

            // Verify payment status
            if (paymentIntent.Status != "succeeded")
            {
                Console.WriteLine($"Payment not successful. Status: {paymentIntent.Status}");
                return false;
            }

            return true; // Payment confirmed successfully
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error confirming payment: {ex.Message}");
            return false;
        }
    }
}
