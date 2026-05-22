using System.Text.Json.Serialization;

namespace StrayCat.Application.DTOs;

public class BeamCheckoutDto
{
    public bool CollectDeliveryAddress { get; set; }
    public bool CollectPhoneNumber { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string FeeType { get; set; }
    public LinkSettingsDto LinkSettings { get; set; }
    public OrderDto Order { get; set; }
    public string RedirectUrl { get; set; }
}

public class LinkSettingsDto
{
    public PaymentMethodDto BuyNowPayLater { get; set; }
    public PaymentMethodDto Card { get; set; }
    public CardInstallmentsDto CardInstallments { get; set; }
    public PaymentMethodDto EWallets { get; set; }
    public PaymentMethodDto MobileBanking { get; set; }
    public PaymentMethodDto QrPromptPay { get; set; }
}

public class PaymentMethodDto
{
    public bool IsEnabled { get; set; }
}

public class CardInstallmentsDto
{
    public bool IsEnabled { get; set; }
    public InstallmentOptionDto Installments3m { get; set; }
    public InstallmentOptionDto Installments4m { get; set; }
    public InstallmentOptionDto Installments6m { get; set; }
    public InstallmentOptionDto Installments10m { get; set; }
}

public class InstallmentOptionDto
{
    public bool IsEnabled { get; set; }
}

public class OrderDto
{
    public string Currency { get; set; }
    public string Description { get; set; }
    public string InternalNote { get; set; }
    public decimal NetAmount { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public string ReferenceId { get; set; }
}

public class OrderItemDto
{
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string ItemName { get; set; }
    public decimal Price { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string Sku { get; set; }
}

public class BeamCheckoutResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
