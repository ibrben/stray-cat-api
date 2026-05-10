using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.Interfaces;
using StrayCat.Application.DTOs;
using StrayCat.Application.Services;

namespace StrayCat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;

    public PaymentController(IPaymentService paymentService, IBookingService bookingService)
    {
        _paymentService = paymentService;
        _bookingService = bookingService;
    }

    [HttpPost("payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        try
        {
            var clientSecret = await _paymentService.CreatePaymentIntentAsync(request.Amount, request.Currency);
            
            return Ok(new { clientSecret });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to create payment intent: {ex.Message}" });
        }
    }

    [HttpPost("confirm-payment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmationRequest request)
    {
        try
        {
            var result = await _bookingService.ConfirmPaymentAsync(request.ConfirmationId, request.BookingId, request.ReferenceCode);
            
            if (result)
            {
                return Ok(new { message = "Payment confirmed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to confirm payment" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to confirm payment: {ex.Message}" });
        }
    }
}
