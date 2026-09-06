using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlogWritingInvitationsController : ControllerBase
    {
        private readonly IBlogWritingInvitationService _invitationService;

        public BlogWritingInvitationsController(IBlogWritingInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        // POST: api/blog-writing-invitations
        [HttpPost]
        public async Task<IActionResult> CreateInvitation([FromBody] CreateBlogWritingInvitationDto dto)
        {
            try
            {
                var userId = GetUserId();
                var invitation = await _invitationService.CreateInvitationAsync(dto, userId);
                return CreatedAtAction(nameof(GetInvitations), new { id = invitation.Id }, invitation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/blog-writing-invitations
        [HttpGet]
        public async Task<IActionResult> GetInvitations()
        {
            try
            {
                var userId = GetUserId();
                var invitations = await _invitationService.GetUserInvitationsAsync(userId);
                return Ok(invitations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving invitations.");
            }
        }

        // DELETE: api/blog-writing-invitations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RevokeInvitation(string id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _invitationService.RevokeInvitationAsync(id, userId);
                
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while revoking the invitation.");
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user token.");
            }
            return userId;
        }
    }
}