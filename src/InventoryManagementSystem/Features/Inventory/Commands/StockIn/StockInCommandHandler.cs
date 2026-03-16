// Features/Inventory/Commands/StockIn/StockInCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Inventory.Commands.StockIn
{
    /// <summary>
    /// Unit of Work Pattern in action:
    ///  1. Begin transaction via IUnitOfWork
    ///  2. Upsert StockLevel (repository)
    ///  3. Insert StockTransaction log (repository)
    ///  4. Commit — both succeed or both roll back atomically
    /// </summary>
    public sealed class StockInCommandHandler : IRequestHandler<StockInCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public StockInCommandHandler(IUnitOfWork uow,
            IStockLevelRepository stockRepo,
            IStockTransactionRepository txnRepo)
        {
            _uow       = uow;
            _stockRepo = stockRepo;
            _txnRepo   = txnRepo;
        }

        public async Task<Result<string>> Handle(StockInCommand cmd, CancellationToken ct)
        {
            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _stockRepo.UpsertAsync(cmd.ProductId, cmd.WarehouseId, cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                var txn = new StockTransaction
                {
                    TransactionId   = Guid.NewGuid().ToString(),
                    ProductId       = cmd.ProductId,
                    WarehouseId     = cmd.WarehouseId,
                    TransactionType = TransactionType.Purchase,
                    Quantity        = cmd.Quantity
                };
                await _txnRepo.InsertAsync(txn, _uow.Connection, _uow.Transaction!, ct);

                await _uow.CommitAsync(ct);
                return Result<string>.Success($"Stock In successful. +{cmd.Quantity} units added.");
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }
    }
}
