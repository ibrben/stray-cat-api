using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces;

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
