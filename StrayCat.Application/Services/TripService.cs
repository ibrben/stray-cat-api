using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Services
{
    public class TripService : ITripService
    {
        private readonly StrayCatDbContext _context;

        public TripService(StrayCatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
        {
            var trips = await _context.Trips
                .Include(t => t.TripDates)
                .Include(t => t.TripTags)
                .Include(t => t.Bookings)
                .Include(t => t.TripImages)
                .Include(t => t.Organizer)
                .ToListAsync();

            return trips.Select(MapToTripDto);
        }

        public async Task<TripDto?> GetTripByIdAsync(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.TripDates)
                .Include(t => t.TripTags)
                .Include(t => t.Bookings)
                .Include(t => t.TripImages)
                .FirstOrDefaultAsync(t => t.Id == id);

            return trip != null ? MapToTripDto(trip) : null;
        }

        public async Task<TripDto> CreateTripAsync(TripDto tripDto, int userId)
        {
            var trip = new Trip
            {
                Title = tripDto.Title,
                Description = tripDto.Description,
                Category = tripDto.Category,
                MaxGuests = tripDto.MaxGuests,
                Price = tripDto.Price,
                ImageUrl = tripDto.ImageUrl,
                Type = tripDto.Type,
                Location = tripDto.Location,
                Duration = string.Empty,
                Currency = tripDto.Currency ?? "THB",
                IsActive = true,
                OrganizerId = userId, // Use the authenticated user's ID
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            // Add TripDates if provided
            if (tripDto.StartDate.HasValue && tripDto.EndDate.HasValue)
            {
                var tripDate = new TripDate
                {
                    TripId = trip.Id,
                    StartDate = tripDto.StartDate.Value.ToUniversalTime(),
                    EndDate = tripDto.EndDate.Value.ToUniversalTime(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.TripDates.Add(tripDate);
            }

            // Add TripTags if provided
            // foreach (var tagName in tripDto.Tags)
            // {
            //     var tripTag = new TripTag
            //     {
            //         TripId = trip.Id,
            //         Name = tagName,
            //         CreatedAt = DateTime.UtcNow,
            //         UpdatedAt = DateTime.UtcNow
            //     };
            //     _context.TripTags.Add(tripTag);
            // }

            await _context.SaveChangesAsync();
            return MapToTripDto(trip);
        }

        public async Task<bool> UpdateTripAsync(int id, TripDto tripDto, int userId)
        {
            var existingTrip = await _context.Trips
                .Include(t => t.TripDates)
                .Include(t => t.TripTags)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (existingTrip == null)
                return false;
            
            // Check if user owns this trip
            if (existingTrip.Organizer.Id != userId)
                return false;

            existingTrip.Title = tripDto.Title;
            existingTrip.Description = tripDto.Description;
            existingTrip.Category = tripDto.Category;
            existingTrip.MaxGuests = tripDto.MaxGuests;
            existingTrip.Price = tripDto.Price;
            existingTrip.ImageUrl = tripDto.ImageUrl;
            existingTrip.Type = tripDto.Type;
            existingTrip.UpdatedAt = DateTime.UtcNow;

            // Update TripDates
            if (tripDto.StartDate.HasValue && tripDto.EndDate.HasValue)
            {
                var existingDate = existingTrip.TripDates.FirstOrDefault();
                if (existingDate != null)
                {
                    existingDate.StartDate = tripDto.StartDate.Value;
                    existingDate.EndDate = tripDto.EndDate.Value;
                    existingDate.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    var newDate = new TripDate
                    {
                        TripId = existingTrip.Id,
                        StartDate = tripDto.StartDate.Value,
                        EndDate = tripDto.EndDate.Value,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.TripDates.Add(newDate);
                }
            }

            // Update TripTags
            _context.TripTags.RemoveRange(existingTrip.TripTags);
            foreach (var tagName in tripDto.Tags)
            {
                var tripTag = new TripTag
                {
                    TripId = existingTrip.Id,
                    Name = tagName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.TripTags.Add(tripTag);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripAsync(int id, int userId)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return false;
            
            // Check if user owns this trip
            if (trip.Organizer.Id != userId)
                return false;

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return true;
        }

        private static TripDto MapToTripDto(Trip trip)
        {
            var firstDate = trip.TripDates.FirstOrDefault();
            var totalBookedGuests = trip.Bookings?.Sum(b => b.GuestCount) ?? 0;
            
            return new TripDto
            {
                TripId = trip.Id,
                Title = trip.Title,
                Description = trip.Description,
                MaxGuests = trip.MaxGuests,
                Price = trip.Price,
                ImageUrl = trip.ImageUrl,
                Type = trip.Type,
                Category = trip.Category,
                StartDate = firstDate?.StartDate,
                EndDate = firstDate?.EndDate,
                Tags = trip.TripTags.Select(t => t.Name).ToList(),
                Location = trip.Location,
                Currency = trip.Currency,
                Organizer = new OrganizerDto
                {
                    Id = trip.Organizer?.Id ?? 0,
                    Name = trip.Organizer?.Name ?? "Unknown Organizer"
                },
                BookedGuest = totalBookedGuests
            };
        }
    }
}
