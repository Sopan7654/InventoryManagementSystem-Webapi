// ============================================================
// FILE: src/InventoryManagement.API/Middleware/RequestLoggingMiddleware.cs
// ============================================================
using System.Diagnostics;

namespace InventoryManagement.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        { _next = next; _logger = logger; }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                ?? Guid.NewGuid().ToString("N")[..12];

            context.Response.Headers["X-Correlation-ID"] = correlationId;

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                Stopwatch sw = Stopwatch.StartNew();
                _logger.LogInformation("→ {Method} {Path} [CID: {CorrelationId}]",
                    context.Request.Method, context.Request.Path, correlationId);

                await _next(context);
                sw.Stop();

                _logger.LogInformation("← {StatusCode} in {ElapsedMs}ms [CID: {CorrelationId}]",
                    context.Response.StatusCode, sw.ElapsedMilliseconds, correlationId);
            }
        }
    }
}
