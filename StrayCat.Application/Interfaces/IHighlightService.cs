using StrayCat.Application.DTOs;

namespace StrayCat.Application.Interfaces
{
    public interface IHighlightService
    {
        Task<CreateHighlightResponseDto> CreateHighlightsAsync(CreateHighlightRequestDto request);
        Task<IEnumerable<HighlightDto>> GetHighlightsByTripIdAsync(int tripId);
    }
}
