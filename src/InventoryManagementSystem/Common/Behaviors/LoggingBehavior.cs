// Common/Behaviors/LoggingBehavior.cs  —  Chain of Responsibility Step 1
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Common.Behaviors
{
    /// <summary>
    /// Chain of Responsibility — Step 1.
    /// Logs every request and its execution time before passing to the next handler.
    /// </summary>
    public sealed class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("[{RequestName}] → Handling", requestName);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            _logger.LogInformation("[{RequestName}] ← Handled in {ElapsedMs}ms",
                requestName, sw.ElapsedMilliseconds);

            return response;
        }
    }
}
