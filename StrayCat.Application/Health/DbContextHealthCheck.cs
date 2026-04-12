using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using StrayCat.Infrastructure.Data;

namespace StrayCat.Application.Health
{
    public class DbContextHealthCheck : IHealthCheck
    {
        private readonly StrayCatDbContext _dbContext;

        public DbContextHealthCheck(StrayCatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if database is reachable
                await _dbContext.Database.CanConnectAsync(cancellationToken);
                
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    $"Database connection failed: {ex.Message}");
            }
        }
    }
}
