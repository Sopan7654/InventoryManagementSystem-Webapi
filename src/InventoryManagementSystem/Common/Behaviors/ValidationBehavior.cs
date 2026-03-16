// Common/Behaviors/ValidationBehavior.cs  —  Chain of Responsibility Step 2
using FluentValidation;
using MediatR;
using ValidationException = InventoryManagementSystem.Common.Exceptions.ValidationException;

namespace InventoryManagementSystem.Common.Behaviors
{
    /// <summary>
    /// Chain of Responsibility — Step 2.
    /// Runs all registered FluentValidation validators for the incoming request.
    /// If any rule fails, throws <see cref="ValidationException"/> (mapped to HTTP 400)
    /// and the actual handler is never invoked.
    /// </summary>
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());

            if (failures.Any())
                throw new ValidationException(failures);

            return await next();
        }
    }
}
