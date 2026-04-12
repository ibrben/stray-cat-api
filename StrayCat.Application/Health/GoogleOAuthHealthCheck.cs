using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StrayCat.Application.Settings;
using System.Net.Http;

namespace StrayCat.Application.Health
{
    public class GoogleOAuthHealthCheck : IHealthCheck
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly HttpClient _httpClient;

        public GoogleOAuthHealthCheck(
            IOptions<GoogleAuthSettings> googleAuthSettings,
            HttpClient httpClient)
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if Google OAuth settings are configured
                if (string.IsNullOrEmpty(_googleAuthSettings.ClientId) || 
                    _googleAuthSettings.ClientId == "your-google-client-id.apps.googleusercontent.com")
                {
                    return HealthCheckResult.Degraded(
                        "Google OAuth ClientId is not configured");
                }

                // Test Google OAuth endpoint availability
                var response = await _httpClient.GetAsync(
                    "https://accounts.google.com/.well-known/openid_configuration", 
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Google OAuth service is available");
                }
                else
                {
                    return HealthCheckResult.Unhealthy(
                        $"Google OAuth service returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    $"Google OAuth health check failed: {ex.Message}");
            }
        }
    }
}
