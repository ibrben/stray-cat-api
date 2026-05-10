using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StrayCat.Application.Common;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<BookingDto?> GetBookingByReferenceCodeAsync(string referenceCode);
        Task<CreateBookingResponse> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<bool> UpdateBookingAsync(int id, BookingDto bookingDto);
        Task<bool> DeleteBookingAsync(int id);
        Task<IEnumerable<BookingDto>> GetBookingsByTripIdAsync(int tripId);
        Task<IEnumerable<BookingDto>> GetBookingsForUserAsync(int userId, string userEmail);
        Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode);
    }

    public class BookingService : IBookingService
    {
        private readonly StrayCatDbContext _context;
        private readonly IReferenceCodeGenerator _referenceCodeGenerator;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public BookingService(StrayCatDbContext context, IReferenceCodeGenerator referenceCodeGenerator, IConfiguration configuration, IPaymentService paymentService)
        {
            _context = context;
            _referenceCodeGenerator = referenceCodeGenerator;
            _configuration = configuration;
            _paymentService = paymentService;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                .ToListAsync();

            return bookings.Select(MapToBookingDto);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking != null ? MapToBookingDto(booking) : null;
        }

        public async Task<BookingDto?> GetBookingByReferenceCodeAsync(string referenceCode)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.ReferenceCode == referenceCode);

            return booking != null ? MapToBookingDto(booking) : null;
        }

        public async Task<CreateBookingResponse> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            // Check if trip exists and has available slots
            var trip = await _context.Trips
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == bookingDto.TripId);

            if (trip == null)
                throw new ArgumentException("Trip not found");

            var totalBookedGuests = trip.Bookings?.Sum(b => b.GuestCount) ?? 0;
            var availableSlots = trip.MaxGuests - totalBookedGuests;

            if (availableSlots < bookingDto.GuestCount)
                throw new InvalidOperationException("Not enough available slots for this trip");

            // Generate unique reference code
            string referenceCode;
            do
            {
                referenceCode = _referenceCodeGenerator.GenerateReferenceCode();
            } while (await _context.Bookings.AnyAsync(b => b.ReferenceCode == referenceCode));

            // Create payment intent
            var paymentIntentClientSecret = await _paymentService.CreatePaymentIntentAsync(bookingDto.TotalPrice, "thb");

            var booking = new Booking
            {
                TripId = bookingDto.TripId,
                ReferenceCode = referenceCode,
                CustomerName = bookingDto.CustomerName,
                CustomerPhoneNo = bookingDto.CustomerPhoneNo,
                CustomerEmail = bookingDto.CustomerEmail,
                GuestCount = bookingDto.GuestCount,
                Notes = bookingDto.Notes,
                TotalPrice = bookingDto.TotalPrice - bookingDto.ServiceFee,
                ServiceFee = bookingDto.ServiceFee,
                GrandTotal = bookingDto.TotalPrice,
                Status = BookingStatus.WaitingForPayment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PaymentIntentClientSecret = paymentIntentClientSecret,
                IdCard = bookingDto.customerIDCard
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var createdBookingDto = await GetBookingByIdAsync(booking.Id);
            
            return new CreateBookingResponse
            {
                Booking = createdBookingDto!,
                PaymentIntentClientSecret = paymentIntentClientSecret
            };
        }

        public async Task<bool> UpdateBookingAsync(int id, BookingDto bookingDto)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
                return false;

            existingBooking.CustomerName = bookingDto.CustomerName;
            existingBooking.CustomerPhoneNo = bookingDto.CustomerPhoneNo;
            existingBooking.CustomerEmail = bookingDto.CustomerEmail;
            existingBooking.GuestCount = bookingDto.GuestCount;
            existingBooking.Notes = bookingDto.Notes;
            existingBooking.TotalPrice = bookingDto.TotalPrice;
            existingBooking.ServiceFee = bookingDto.ServiceFee;
            existingBooking.GrandTotal = bookingDto.TotalPrice + bookingDto.ServiceFee;
            existingBooking.Status = bookingDto.Status;
            existingBooking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmPaymentAsync(string confirmationId, int bookingId, string referenceCode)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.Id == bookingId && b.ReferenceCode == referenceCode);

                if (booking == null)
                {
                    Console.WriteLine($"Booking not found with ID: {bookingId} and reference code: {referenceCode}");
                    return false;
                }

                // Check if booking status is "Waiting for Payment"
                if (booking.Status != BookingStatus.WaitingForPayment)
                {
                    Console.WriteLine($"Booking status is not waiting for payment. Current status: {booking.Status}");
                    return false;
                }

                // Check if paymentIntentClientSecret exists
                if (string.IsNullOrEmpty(booking.PaymentIntentClientSecret))
                {
                    Console.WriteLine($"Payment intent client secret is missing for booking ID: {bookingId}");
                    return false;
                }

                // Verify payment with Stripe first
                var paymentConfirmed = await _paymentService.ConfirmPaymentAsync(confirmationId, bookingId, referenceCode);
                if (!paymentConfirmed)
                {
                    Console.WriteLine($"Payment verification failed for confirmation ID: {confirmationId}");
                    return false;
                }

                // Update booking status and store confirmation ID
                booking.Status = BookingStatus.Confirmed;
                booking.PaymentConfirmationId = confirmationId;
                booking.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                Console.WriteLine($"Payment confirmed successfully for booking ID: {bookingId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming payment: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByTripIdAsync(int tripId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.TripId == tripId)
                .ToListAsync();

            return bookings.Select(MapToBookingDto);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsForUserAsync(int userId, string userEmail)
        {
            // Get admin email list from configuration
            var adminEmails = _configuration.GetSection("AdminSettings:AdminEmails").Get<List<string>>() ?? new List<string>();
            
            // Check if user is admin
            var isAdmin = adminEmails.Contains(userEmail, StringComparer.OrdinalIgnoreCase);
            
            IQueryable<Booking> query;
            
            if (isAdmin)
            {
                // Admin can see all bookings
                query = _context.Bookings
                    .Include(b => b.Trip)
                    .ThenInclude(t => t.Organizer);
            }
            else
            {
                // Organizer can only see bookings for their own trips
                query = _context.Bookings
                    .Include(b => b.Trip)
                    .ThenInclude(t => t.Organizer)
                    .Where(b => b.Trip.OrganizerId == userId);
            }
            
            var bookings = await query.ToListAsync();
            return bookings.Select(MapToBookingDto);
        }

        private static BookingDto MapToBookingDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                TripId = booking.TripId,
                ReferenceCode = booking.ReferenceCode,
                CustomerName = booking.CustomerName,
                CustomerPhoneNo = booking.CustomerPhoneNo,
                CustomerEmail = booking.CustomerEmail,
                GuestCount = booking.GuestCount,
                Notes = booking.Notes,
                TotalPrice = booking.TotalPrice,
                ServiceFee = booking.ServiceFee,
                GrandTotal = booking.GrandTotal,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                PaymentIntentClientSecret = booking.PaymentIntentClientSecret,
                IdCard = booking.IdCard,
                Trip = booking.Trip != null ? new TripSummaryDto
                {
                    TripId = booking.Trip.Id,
                    Title = booking.Trip.Title,
                    ImageUrl = booking.Trip.ImageUrl,
                    StartDate = booking.Trip.TripDates.FirstOrDefault()?.StartDate,
                    EndDate = booking.Trip.TripDates.FirstOrDefault()?.EndDate
                } : null
            };
        }
    }
}
