using System.Diagnostics;

namespace StrayCat.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var method = context.Request.Method;
            var path = context.Request.Path;
            var host = context.Request.Host.Value;

            _logger.LogInformation("Request from {RemoteIp} - {Method} {Host}{Path}", remoteIp, method, host, path);

            await _next(context);
        }
    }
}
