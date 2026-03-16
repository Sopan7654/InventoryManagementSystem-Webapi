// Features/Inventory/Commands/Transfer/TransferCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Inventory.Commands.Transfer
{
    public sealed class TransferCommandHandler : IRequestHandler<TransferCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public TransferCommandHandler(IUnitOfWork uow, IStockLevelRepository stockRepo, IStockTransactionRepository txnRepo)
        { _uow = uow; _stockRepo = stockRepo; _txnRepo = txnRepo; }

        public async Task<Result<string>> Handle(TransferCommand cmd, CancellationToken ct)
        {
            var fromStock = await _stockRepo.GetByProductAndWarehouseAsync(cmd.ProductId, cmd.FromWarehouseId, ct)
                ?? throw new DomainException($"No stock in source warehouse '{cmd.FromWarehouseId}' for product '{cmd.ProductId}'.");

            if (fromStock.AvailableQuantity < cmd.Quantity)
                throw new DomainException($"Insufficient stock in source. Available: {fromStock.AvailableQuantity:N2}");

            var reference = $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // Deduct from source
                await _stockRepo.UpdateQuantityAsync(cmd.ProductId, cmd.FromWarehouseId, -cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                // Add to destination (upsert — create record if not exists)
                await _stockRepo.UpsertAsync(cmd.ProductId, cmd.ToWarehouseId, cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                // Log both legs of the transfer
                await _txnRepo.InsertAsync(new StockTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(), ProductId = cmd.ProductId,
                    WarehouseId = cmd.FromWarehouseId, TransactionType = TransactionType.TransferOut,
                    Quantity = cmd.Quantity, Reference = reference
                }, _uow.Connection, _uow.Transaction!, ct);

                await _txnRepo.InsertAsync(new StockTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(), ProductId = cmd.ProductId,
                    WarehouseId = cmd.ToWarehouseId, TransactionType = TransactionType.TransferIn,
                    Quantity = cmd.Quantity, Reference = reference
                }, _uow.Connection, _uow.Transaction!, ct);

                await _uow.CommitAsync(ct);
                return Result<string>.Success($"Transfer complete. {cmd.Quantity} units moved. Ref: {reference}");
            }
            catch { await _uow.RollbackAsync(ct); throw; }
        }
    }
}
