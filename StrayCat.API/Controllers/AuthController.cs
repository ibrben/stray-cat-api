using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StrayCat.Application.Services;
using StrayCat.Application.DTOs;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                    return BadRequest("Email and password are required.");

                var response = await _authService.LoginAsync(loginRequest);
                
                if (response == null)
                    return Unauthorized("Invalid email or password.");

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, "An error occurred during login.");
            }
        }

        // POST: /api/auth/register (placeholder - not implemented in IAuthService)
        [HttpPost("register")]
        public IActionResult Register()
        {
            return BadRequest("Registration not implemented in current AuthService.");
        }

        // POST: /api/auth/refresh (placeholder - not implemented in IAuthService)
        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            return BadRequest("Token refresh not implemented in current AuthService.");
        }

        // POST: /api/auth/logout (placeholder - not implemented in IAuthService)
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return BadRequest("Logout not implemented in current AuthService.");
        }

        // GET: /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return BadRequest("User ID not found in token.");

                var userId = int.Parse(userIdClaim.Value);
                var user = await _authService.GetUserByIdAsync(userId);
                
                if (user == null)
                    return NotFound("User not found.");

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, "An error occurred while fetching user information.");
            }
        }
    }
}
