using System;
using System.Diagnostics;
using System.Text;

namespace ApplicationService.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            _logger.LogInformation("Request, {url} Response content length {contentLength}", context.Request.Path, context.Response.ContentLength);
        }
    }
}
