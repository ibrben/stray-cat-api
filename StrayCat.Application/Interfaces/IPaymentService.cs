using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency = "thb");
    Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode);
    Task<string> CreatePaymentUrl(string productName, int productId, string refCode, decimal amount, decimal unitPrice, int quantity, string imgUrl);
}
