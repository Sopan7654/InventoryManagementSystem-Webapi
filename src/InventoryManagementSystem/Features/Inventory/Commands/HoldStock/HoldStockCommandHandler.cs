// Features/Inventory/Commands/HoldStock/HoldStockCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Inventory.Commands.HoldStock
{
    public sealed class HoldStockCommandHandler : IRequestHandler<HoldStockCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public HoldStockCommandHandler(IUnitOfWork uow, IStockLevelRepository stockRepo, IStockTransactionRepository txnRepo)
        { _uow = uow; _stockRepo = stockRepo; _txnRepo = txnRepo; }

        public async Task<Result<string>> Handle(HoldStockCommand cmd, CancellationToken ct)
        {
            var stock = await _stockRepo.GetByProductAndWarehouseAsync(cmd.ProductId, cmd.WarehouseId, ct)
                ?? throw new DomainException("No stock record found for this product/warehouse combination.");

            if (stock.AvailableQuantity < cmd.Quantity)
                throw new DomainException($"Cannot hold {cmd.Quantity:N2} units. Available (unreserved): {stock.AvailableQuantity:N2}");

            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _stockRepo.UpdateReservedAsync(cmd.ProductId, cmd.WarehouseId, cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                await _txnRepo.InsertAsync(new StockTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(), ProductId = cmd.ProductId,
                    WarehouseId = cmd.WarehouseId, TransactionType = TransactionType.Hold, Quantity = cmd.Quantity
                }, _uow.Connection, _uow.Transaction!, ct);

                await _uow.CommitAsync(ct);
                return Result<string>.Success($"{cmd.Quantity} units reserved/held successfully.");
            }
            catch { await _uow.RollbackAsync(ct); throw; }
        }
    }
}
