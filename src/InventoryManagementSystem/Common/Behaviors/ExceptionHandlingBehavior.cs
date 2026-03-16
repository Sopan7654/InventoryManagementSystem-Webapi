// Common/Behaviors/ExceptionHandlingBehavior.cs  —  Chain of Responsibility Step 3
using MediatR;
using InventoryManagementSystem.Common.Exceptions;

namespace InventoryManagementSystem.Common.Behaviors
{
    /// <summary>
    /// Chain of Responsibility — Step 3 (innermost before the real handler).
    /// Catches any unhandled exception from the handler and re-throws as a
    /// <see cref="DomainException"/> so the GlobalExceptionMiddleware can map it cleanly.
    /// </summary>
    public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (DomainException)    { throw; }
            catch (NotFoundException)  { throw; }
            catch (ValidationException){ throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception processing {RequestName}", typeof(TRequest).Name);
                throw new DomainException(
                    $"An unexpected error occurred while processing {typeof(TRequest).Name}.", ex);
            }
        }
    }
}
