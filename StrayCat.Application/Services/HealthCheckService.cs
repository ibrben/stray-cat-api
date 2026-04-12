using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StrayCat.Application.Settings;
using StrayCat.Application.Health;
using StrayCat.Infrastructure.Data;
using System.Net.Http;
using System.Collections.Generic;

namespace StrayCat.Application.Services
{
    public interface IHealthCheckService
    {
        Task<CustomHealthReport> CheckHealthAsync();
    }

    public class CustomHealthReport
    {
        public string Status { get; set; } = string.Empty;
        public List<HealthCheckResult> Checks { get; set; } = new();
    }

    public class HealthCheckResult
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Duration { get; set; }
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly StrayCatDbContext _dbContext;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly HttpClient _httpClient;

        public HealthCheckService(
            StrayCatDbContext dbContext,
            IOptions<GoogleAuthSettings> googleAuthSettings,
            HttpClient httpClient)
        {
            _dbContext = dbContext;
            _googleAuthSettings = googleAuthSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<CustomHealthReport> CheckHealthAsync()
        {
            var checks = new List<HealthCheckResult>();

            // Database health check
            var dbStartTime = DateTime.UtcNow;
            try
            {
                await _dbContext.Database.CanConnectAsync();
                checks.Add(new HealthCheckResult
                {
                    Name = "database",
                    Status = "Healthy",
                    Description = "Database connection is healthy",
                    Duration = (DateTime.UtcNow - dbStartTime).TotalMilliseconds
                });
            }
            catch (Exception ex)
            {
                checks.Add(new HealthCheckResult
                {
                    Name = "database",
                    Status = "Unhealthy",
                    Description = $"Database connection failed: {ex.Message}",
                    Duration = (DateTime.UtcNow - dbStartTime).TotalMilliseconds
                });
            }

            // Google OAuth health check
            var oauthStartTime = DateTime.UtcNow;
            try
            {
                if (string.IsNullOrEmpty(_googleAuthSettings.ClientId) || 
                    _googleAuthSettings.ClientId == "your-google-client-id.apps.googleusercontent.com")
                {
                    checks.Add(new HealthCheckResult
                    {
                        Name = "google-oauth",
                        Status = "Degraded",
                        Description = "Google OAuth ClientId is not configured",
                        Duration = (DateTime.UtcNow - oauthStartTime).TotalMilliseconds
                    });
                }
                else
                {
                    var response = await _httpClient.GetAsync(
                        "https://accounts.google.com/.well-known/openid_configuration");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        checks.Add(new HealthCheckResult
                        {
                            Name = "google-oauth",
                            Status = "Healthy",
                            Description = "Google OAuth service is available",
                            Duration = (DateTime.UtcNow - oauthStartTime).TotalMilliseconds
                        });
                    }
                    else
                    {
                        checks.Add(new HealthCheckResult
                        {
                            Name = "google-oauth",
                            Status = "Unhealthy",
                            Description = $"Google OAuth service returned {response.StatusCode}",
                            Duration = (DateTime.UtcNow - oauthStartTime).TotalMilliseconds
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                checks.Add(new HealthCheckResult
                {
                    Name = "google-oauth",
                    Status = "Unhealthy",
                    Description = $"Google OAuth health check failed: {ex.Message}",
                    Duration = (DateTime.UtcNow - oauthStartTime).TotalMilliseconds
                });
            }

            // Self health check
            checks.Add(new HealthCheckResult
            {
                Name = "self",
                Status = "Healthy",
                Description = "API is running",
                Duration = 1.0
            });

            // Calculate overall status
            var overallStatus = checks.Any(c => c.Status == "Unhealthy") 
                ? "Unhealthy" 
                : checks.Any(c => c.Status == "Degraded") 
                    ? "Degraded" 
                    : "Healthy";

            return new CustomHealthReport
            {
                Status = overallStatus,
                Checks = checks
            };
        }
    }
}
