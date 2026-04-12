using Microsoft.AspNetCore.Mvc;
using StrayCat.Application.Services;
using System.Net.Mime;

namespace StrayCat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealth()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            var response = new
            {
                status = report.Status,
                checks = report.Checks.Select(check => new
                {
                    name = check.Name,
                    status = check.Status,
                    description = check.Description,
                    duration = check.Duration
                })
            };

            return Ok(response);
        }

        [HttpGet("health/ready")]
        public IActionResult GetReadiness()
        {
            return Ok(new { status = "Ready" });
        }

        [HttpGet("health/live")]
        public IActionResult GetLiveness()
        {
            return Ok(new { status = "Alive" });
        }
    }

    public class HealthCheckResponse
    {
        public string status { get; set; } = string.Empty;
        public HealthCheckDetail[] checks { get; set; } = Array.Empty<HealthCheckDetail>();
    }

    public class HealthCheckDetail
    {
        public string name { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public double duration { get; set; }
    }
}
