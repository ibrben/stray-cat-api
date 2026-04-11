using StrayCat.Application.DTOs;

namespace StrayCat.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        Task<GoogleAuthResponseDto?> AuthenticateWithGoogleAsync(GoogleAuthRequestDto request);
        Task<UserDto?> GetUserByGoogleIdAsync(string googleId);
    }
}
