using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.DTOs;
using StrayCat.Application.Interfaces;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/guest-blog-writing")]
    public class GuestBlogWritingController : ControllerBase
    {
        private readonly IBlogWritingInvitationService _invitationService;

        public GuestBlogWritingController(IBlogWritingInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        // GET: api/guest-blog-writing/{token}
        [HttpGet("{token}")]
        public async Task<IActionResult> GetGuestContext(string token)
        {
            try
            {
                var context = await _invitationService.GetGuestContextAsync(token);
                
                if (context == null)
                {
                    return NotFound();
                }

                return Ok(context);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving guest context.");
            }
        }

        // POST: api/guest-blog-writing/{token}/blogs
        [HttpPost("{token}/blogs")]
        public async Task<IActionResult> SubmitGuestBlog(string token, [FromBody] GuestBlogSubmissionDto dto)
        {
            try
            {
                var result = await _invitationService.SubmitGuestBlogAsync(token, dto);
                
                if (result == null)
                {
                    return NotFound();
                }

                return CreatedAtAction(nameof(SubmitGuestBlog), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while submitting the guest blog.");
            }
        }
    }
}