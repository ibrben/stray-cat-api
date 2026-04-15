using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Services
{
    public interface ITripImageService
    {
        Task<IEnumerable<TripImageDto>> GetTripImagesAsync(int tripId);
        Task<TripImageDto?> GetTripImageByIdAsync(int id);
        Task<TripImageDto> CreateTripImageAsync(CreateTripImageDto imageDto);
        Task<bool> UpdateTripImageAsync(int id, TripImageDto imageDto);
        Task<bool> DeleteTripImageAsync(int id);
        Task<bool> UpdateCoverImageAsync(int tripId, string cdnUrl);
        Task<List<TripImageDto>> AddMultipleImagesAsync(MultipleImagesRequestDto request);
    }

    public class TripImageService : ITripImageService
    {
        private readonly StrayCatDbContext _context;

        public TripImageService(StrayCatDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TripImageDto>> GetTripImagesAsync(int tripId)
        {
            var images = await _context.TripImages
                .Where(ti => ti.TripId == tripId)
                .OrderBy(ti => ti.DisplayOrder)
                .ToListAsync();

            return images.Select(MapToTripImageDto);
        }

        public async Task<TripImageDto?> GetTripImageByIdAsync(int id)
        {
            var image = await _context.TripImages.FindAsync(id);
            return image != null ? MapToTripImageDto(image) : null;
        }

        public async Task<TripImageDto> CreateTripImageAsync(CreateTripImageDto imageDto)
        {
            var tripImage = new TripImage
            {
                TripId = imageDto.TripId,
                ImageUrl = imageDto.ImageUrl,
                DisplayOrder = imageDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TripImages.Add(tripImage);
            await _context.SaveChangesAsync();

            return await GetTripImageByIdAsync(tripImage.Id);
        }

        public async Task<bool> UpdateTripImageAsync(int id, TripImageDto imageDto)
        {
            var existingImage = await _context.TripImages.FindAsync(id);
            if (existingImage == null)
                return false;

            existingImage.ImageUrl = imageDto.ImageUrl;
            existingImage.DisplayOrder = imageDto.DisplayOrder;
            existingImage.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripImageAsync(int id)
        {
            var image = await _context.TripImages.FindAsync(id);
            if (image == null)
                return false;

            _context.TripImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCoverImageAsync(int tripId, string cdnUrl)
        {
            try
            {
                // Find the trip to update
                var trip = await _context.Trips.FindAsync(tripId);
                if (trip == null)
                    return false;

                // Update the trip's cover image URL
                trip.ImageUrl = cdnUrl;
                trip.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<TripImageDto>> AddMultipleImagesAsync(MultipleImagesRequestDto request)
        {
            var addedImages = new List<TripImageDto>();

            try
            {
                foreach (var imageUrlDto in request.ImageUrls)
                {
                    var tripImage = new TripImage
                    {
                        TripId = request.TripId,
                        ImageUrl = imageUrlDto.Url,
                        DisplayOrder = imageUrlDto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.TripImages.Add(tripImage);
                    await _context.SaveChangesAsync();

                    addedImages.Add(MapToTripImageDto(tripImage));
                }

                return addedImages;
            }
            catch (Exception)
            {
                // In case of error, return what was successfully added
                return addedImages;
            }
        }

        private static TripImageDto MapToTripImageDto(TripImage tripImage)
        {
            return new TripImageDto
            {
                Id = tripImage.Id,
                TripId = tripImage.TripId,
                ImageUrl = tripImage.ImageUrl,
                DisplayOrder = tripImage.DisplayOrder,
                CreatedAt = tripImage.CreatedAt,
                UpdatedAt = tripImage.UpdatedAt
            };
        }
    }
}
