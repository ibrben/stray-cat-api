using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.DTOs;
using StrayCat.Application.Services;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // GET: api/bookings/reference/{referenceCode}
        [HttpGet("reference/{referenceCode}")]
        public async Task<IActionResult> GetBookingByReferenceCode(string referenceCode)
        {
            var booking = await _bookingService.GetBookingByReferenceCodeAsync(referenceCode);
            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // GET: api/bookings/trip/{tripId}
        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetBookingsByTripId(int tripId)
        {
            var bookings = await _bookingService.GetBookingsByTripIdAsync(tripId);
            return Ok(bookings);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(bookingDto);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto bookingDto)
        {
            var success = await _bookingService.UpdateBookingAsync(id, bookingDto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var success = await _bookingService.DeleteBookingAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
