using Microsoft.EntityFrameworkCore;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Services
{
    public class HighlightService : IHighlightService
    {
        private readonly StrayCatDbContext _context;

        public HighlightService(StrayCatDbContext context)
        {
            _context = context;
        }

        public async Task<CreateHighlightResponseDto> CreateHighlightsAsync(CreateHighlightRequestDto request)
        {
            // Verify that the trip exists
            var trip = await _context.Trips.FindAsync(request.TripId);
            if (trip == null)
            {
                return new CreateHighlightResponseDto
                {
                    Highlights = new List<HighlightDto>(),
                    Message = $"Trip with ID {request.TripId} not found."
                };
            }

            var createdHighlights = new List<Highlight>();
            var now = DateTime.UtcNow;

            foreach (var item in request.Items)
            {
                var highlight = new Highlight
                {
                    TripId = request.TripId,
                    Item = item,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                
                _context.Highlights.Add(highlight);
                createdHighlights.Add(highlight);
            }

            await _context.SaveChangesAsync();

            var highlightDtos = createdHighlights.Select(h => new HighlightDto
            {
                Id = h.Id,
                TripId = h.TripId,
                Item = h.Item,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            }).ToList();

            return new CreateHighlightResponseDto
            {
                Highlights = highlightDtos,
                Message = $"Successfully created {highlightDtos.Count} highlights for trip {request.TripId}."
            };
        }

        public async Task<IEnumerable<HighlightDto>> GetHighlightsByTripIdAsync(int tripId)
        {
            var highlights = await _context.Highlights
                .Where(h => h.TripId == tripId)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();

            return highlights.Select(h => new HighlightDto
            {
                Id = h.Id,
                TripId = h.TripId,
                Item = h.Item,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            });
        }
    }
}
