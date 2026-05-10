namespace StrayCat.Application.DTOs;

public class CreatePaymentIntentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "thb";
}

public class PaymentConfirmationRequest
{
    public string ConfirmationId { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
}
