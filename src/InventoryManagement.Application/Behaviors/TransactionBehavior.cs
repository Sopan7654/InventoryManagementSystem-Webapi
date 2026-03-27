// ============================================================
// FILE: src/InventoryManagement.Application/Behaviors/TransactionBehavior.cs
// ============================================================
using InventoryManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Behaviors
{
    /// <summary>
    /// Wraps command execution in a database transaction. Only applies to commands (not queries).
    /// </summary>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(IUnitOfWork unitOfWork,
            ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;

            // Only wrap Commands in transactions (skip queries)
            if (!requestName.EndsWith("Command"))
                return await next();

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                _logger.LogDebug("Transaction started for {RequestName}", requestName);

                TResponse response = await next();

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogDebug("Transaction committed for {RequestName}", requestName);

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Transaction rolled back for {RequestName}", requestName);
                throw;
            }
        }
    }
}
