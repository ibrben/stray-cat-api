using StrayCat.Application.Interfaces;
using StrayCat.Application.Settings;
using Microsoft.AspNetCore.Http;

namespace StrayCat.Application.Services
{
    public class UrlService : IUrlService
    {
        private readonly FrontendSettings _frontendSettings;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(
            FrontendSettings frontendSettings, 
            GoogleAuthSettings googleAuthSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _frontendSettings = frontendSettings;
            _googleAuthSettings = googleAuthSettings;
            _httpContextAccessor = httpContextAccessor;
        }

        public string BuildAuthCallbackUrl(string token)
        {
            return $"{_frontendSettings.Url}/auth/callback?token={Uri.EscapeDataString(token)}";
        }

        public string BuildErrorUrl(string errorMessage)
        {
            return $"{_frontendSettings.Url}/auth/error?error={Uri.EscapeDataString(errorMessage)}";
        }

        public string BuildGoogleAuthUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("HTTP context is not available");

            var redirectUri = $"{_googleAuthSettings.CallbackUri}/auth/google/callback";
            var scope = "openid email profile";
            
            return $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={_googleAuthSettings.ClientId}&" +
                $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                $"response_type=code&" +
                $"scope={Uri.EscapeDataString(scope)}&" +
                $"access_type=offline&" +
                $"prompt=consent";
        }
    }
}
