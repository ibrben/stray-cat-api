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
