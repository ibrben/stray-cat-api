namespace StrayCat.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency = "thb");
    Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode);
}
