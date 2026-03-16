// Features/Inventory/Commands/StockOut/StockOutCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Inventory.Commands.StockOut
{
    public sealed class StockOutCommandHandler : IRequestHandler<StockOutCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public StockOutCommandHandler(IUnitOfWork uow, IStockLevelRepository stockRepo, IStockTransactionRepository txnRepo)
        { _uow = uow; _stockRepo = stockRepo; _txnRepo = txnRepo; }

        public async Task<Result<string>> Handle(StockOutCommand cmd, CancellationToken ct)
        {
            // Domain validation: check available stock before opening transaction
            var stock = await _stockRepo.GetByProductAndWarehouseAsync(cmd.ProductId, cmd.WarehouseId, ct)
                ?? throw new DomainException($"No stock record found for product '{cmd.ProductId}' in warehouse '{cmd.WarehouseId}'.");

            if (stock.AvailableQuantity < cmd.Quantity)
                throw new DomainException($"Insufficient stock. Available: {stock.AvailableQuantity:N2}, Requested: {cmd.Quantity:N2}");

            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _stockRepo.UpdateQuantityAsync(cmd.ProductId, cmd.WarehouseId, -cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                await _txnRepo.InsertAsync(new StockTransaction
                {
                    TransactionId   = Guid.NewGuid().ToString(),
                    ProductId       = cmd.ProductId,
                    WarehouseId     = cmd.WarehouseId,
                    TransactionType = TransactionType.Sale,
                    Quantity        = cmd.Quantity
                }, _uow.Connection, _uow.Transaction!, ct);

                await _uow.CommitAsync(ct);
                return Result<string>.Success($"Stock Out successful. -{cmd.Quantity} units removed.");
            }
            catch { await _uow.RollbackAsync(ct); throw; }
        }
    }
}
