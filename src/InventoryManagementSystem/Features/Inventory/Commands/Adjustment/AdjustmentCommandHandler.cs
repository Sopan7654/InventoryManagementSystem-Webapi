// Features/Inventory/Commands/Adjustment/AdjustmentCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Inventory.Commands.Adjustment
{
    public sealed class AdjustmentCommandHandler : IRequestHandler<AdjustmentCommand, Result<string>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public AdjustmentCommandHandler(IUnitOfWork uow, IStockLevelRepository stockRepo, IStockTransactionRepository txnRepo)
        { _uow = uow; _stockRepo = stockRepo; _txnRepo = txnRepo; }

        public async Task<Result<string>> Handle(AdjustmentCommand cmd, CancellationToken ct)
        {
            var stock = await _stockRepo.GetByProductAndWarehouseAsync(cmd.ProductId, cmd.WarehouseId, ct)
                ?? throw new DomainException("No stock record found for this product/warehouse combination.");

            var projected = stock.QuantityOnHand + cmd.Quantity;
            if (projected < 0)
                throw new DomainException($"Adjustment would result in negative stock ({projected:N2}). Current on-hand: {stock.QuantityOnHand:N2}");

            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _stockRepo.UpdateQuantityAsync(cmd.ProductId, cmd.WarehouseId, cmd.Quantity,
                    _uow.Connection, _uow.Transaction!, ct);

                await _txnRepo.InsertAsync(new StockTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(), ProductId = cmd.ProductId,
                    WarehouseId = cmd.WarehouseId, TransactionType = TransactionType.Adjustment,
                    Quantity = cmd.Quantity, Reference = cmd.Reason
                }, _uow.Connection, _uow.Transaction!, ct);

                await _uow.CommitAsync(ct);
                var sign = cmd.Quantity >= 0 ? "+" : "";
                return Result<string>.Success($"Adjustment applied: {sign}{cmd.Quantity} units. New on-hand: {projected:N2}");
            }
            catch { await _uow.RollbackAsync(ct); throw; }
        }
    }
}
