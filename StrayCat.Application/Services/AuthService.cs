using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using StrayCat.Application.DTOs;
using StrayCat.Domain.Entities;
using StrayCat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StrayCat.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly StrayCatDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(StrayCatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            // Find user by email
            var user = await _context.Organizers
                .FirstOrDefaultAsync(o => o.Email.ToLower() == loginRequest.Email.ToLower());

            if (user == null)
                return null;

            // Verify password
            if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
                return null;

            // Check if user is active
            if (!user.IsActive)
                return null;

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    AvatarInitial = user.AvatarInitial,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    MobilePhone = user.MobilePhone,
                    IsVerified = user.IsVerified,
                    IsActive = user.IsActive
                }
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Organizers.FindAsync(userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                AvatarInitial = user.AvatarInitial,
                ProfilePictureUrl = user.ProfilePictureUrl,
                MobilePhone = user.MobilePhone,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract salt from stored hash (assuming format: salt:hash)
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            // Hash the provided password with the same salt
            var hashedInput = HashPassword(password, salt);
            
            return hashedInput == storedHash;
        }

        private string HashPassword(string password, byte[] salt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private string GenerateJwtToken(Organizer user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<GoogleAuthResponseDto?> AuthenticateWithGoogleAsync(GoogleAuthRequestDto request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Code))
                    throw new ArgumentException("Authorization code is required.");

                // Exchange authorization code for tokens
                var tokenResponse = await ExchangeCodeForTokensAsync(request.Code);
                if (tokenResponse == null)
                    throw new InvalidOperationException("Failed to exchange authorization code for tokens.");

                // Validate ID token and get user info
                var payload = await ValidateGoogleIdTokenAsync(tokenResponse.Id_token);
                if (payload == null)
                    throw new InvalidOperationException("Invalid Google ID token.");

                // Validate email from Google
                if (string.IsNullOrWhiteSpace(payload.Email))
                    throw new InvalidOperationException("Google account does not have a valid email address.");

                // Find user by email
                var user = await _context.Organizers
                    .FirstOrDefaultAsync(o => o.Email.ToLower() == payload.Email.ToLower());

                if (user == null)
                    return null; // User doesn't exist - return 401

                // Check if user is active
                if (!user.IsActive)
                    return null; // Inactive user - return 401

                // Update user's Google info if not already set
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = payload.Subject;
                    user.ProfilePictureUrl ??= payload.Picture;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new GoogleAuthResponseDto
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        AvatarInitial = user.AvatarInitial,
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        MobilePhone = user.MobilePhone,
                        IsVerified = user.IsVerified,
                        IsActive = user.IsActive
                    }
                };
            }
            catch (ArgumentException ex)
            {
                // Input validation errors
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                // Google API or authentication errors
                throw new InvalidOperationException($"Authentication error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Unexpected errors
                throw new InvalidOperationException($"An unexpected error occurred during Google authentication: {ex.Message}", ex);
            }
        }

        public async Task<UserDto?> GetUserByGoogleIdAsync(string googleId)
        {
            var user = await _context.Organizers
                .FirstOrDefaultAsync(o => o.GoogleId == googleId);
            
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                AvatarInitial = user.AvatarInitial,
                ProfilePictureUrl = user.ProfilePictureUrl,
                MobilePhone = user.MobilePhone,
                IsVerified = user.IsVerified,
                IsActive = user.IsActive
            };
        }

        private async Task<GoogleTokenResponseDto?> ExchangeCodeForTokensAsync(string code)
        {
            using var httpClient = new HttpClient();
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            
            // Use a fixed redirect URI that matches what we registered with Google
            var redirectUri = "http://localhost:5169/auth/google/callback"; // Update this to match your actual API URL
            
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var parameters = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", clientId!},
                {"client_secret", clientSecret!},
                {"redirect_uri", redirectUri},
                {"grant_type", "authorization_code"}
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(tokenEndpoint, content);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponseDto>(json);
            return tokenResponse;
        }

        private async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleIdTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
    }
}
