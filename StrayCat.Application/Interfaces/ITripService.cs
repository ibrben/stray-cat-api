using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces
{
    public interface ITripService
    {
        Task<IEnumerable<TripDto>> GetAllTripsAsync();
        Task<TripDto?> GetTripByIdAsync(int id);
        Task<TripDto> CreateTripAsync(TripDto trip, int userId);
        Task<bool> UpdateTripAsync(int id, TripDto trip, int userId);
        Task<bool> DeleteTripAsync(int id, int userId);
    }
}
