using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HighlightsController : ControllerBase
    {
        private readonly IHighlightService _highlightService;

        public HighlightsController(IHighlightService highlightService)
        {
            _highlightService = highlightService;
        }

        // POST: api/highlights
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateHighlights([FromBody] CreateHighlightRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _highlightService.CreateHighlightsAsync(request);
            
            if (!result.Highlights.Any())
            {
                return NotFound(result.Message);
            }

            return CreatedAtAction(nameof(GetHighlightsByTripId), new { tripId = request.TripId }, result);
        }

        // GET: api/highlights/trip/{tripId}
        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetHighlightsByTripId(int tripId)
        {
            var highlights = await _highlightService.GetHighlightsByTripIdAsync(tripId);
            return Ok(highlights);
        }
    }
}
