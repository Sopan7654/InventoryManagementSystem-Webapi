// ============================================================
// FILE: src/InventoryManagement.Application/Behaviors/LoggingBehavior.cs
// ============================================================
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that logs every request with timing.
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;
            _logger.LogInformation("▶ Handling {RequestName}", requestName);

            Stopwatch sw = Stopwatch.StartNew();
            TResponse response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
                _logger.LogWarning("⚠ Long-running request: {RequestName} took {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("✓ Handled {RequestName} in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);

            return response;
        }
    }
}
