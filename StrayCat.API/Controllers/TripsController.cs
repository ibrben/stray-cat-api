using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;
using System.Security.Claims;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        // GET: api/trips
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        // GET: api/trips/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound();
            return Ok(trip);
        }

        // POST: api/trips
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTrip([FromBody] TripDto trip)
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            
            var createdTrip = await _tripService.CreateTripAsync(trip, userId);
            return CreatedAtAction(nameof(GetTrip), new { id = createdTrip.TripId }, createdTrip);
        }

        // PUT: api/trips/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] TripDto trip)
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            
            try
            {
                var result = await _tripService.UpdateTripAsync(id, trip, userId);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/trips/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            
            var result = await _tripService.DeleteTripAsync(id, userId);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // POST: api/trips/publish
        [HttpPost("publish")]
        [Authorize]
        public async Task<IActionResult> PublishTrip([FromBody] PublishTripRequestDto request)
        {
            // Get current user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User not found in token.");

            var userId = int.Parse(userIdClaim.Value);
            
            var result = await _tripService.PublishTripAsync(request, userId);
            
            // Check if there was an error
            if (result.Message.Contains("not found") || result.Message.Contains("permission"))
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result.Message);
                return BadRequest(result.Message);
            }
            
            return Ok(result);
        }
    }
}
