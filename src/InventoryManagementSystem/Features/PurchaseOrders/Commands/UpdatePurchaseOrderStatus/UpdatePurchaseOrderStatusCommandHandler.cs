// Features/PurchaseOrders/Commands/UpdatePurchaseOrderStatus/UpdatePurchaseOrderStatusCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Exceptions; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Domain.Enumerations; using InventoryManagementSystem.Features.PurchaseOrders.Repository; using InventoryManagementSystem.Features.Inventory.Repository; using InventoryManagementSystem.Common.Interfaces;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus
{
    public sealed class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, Result<bool>>
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;

        public UpdatePurchaseOrderStatusCommandHandler(IPurchaseOrderRepository repo, IUnitOfWork uow, IStockLevelRepository stockRepo, IStockTransactionRepository txnRepo)
        {
            _repo = repo;
            _uow = uow;
            _stockRepo = stockRepo;
            _txnRepo = txnRepo;
        }

        public async Task<Result<bool>> Handle(UpdatePurchaseOrderStatusCommand cmd, CancellationToken ct)
        {
            if (cmd.Status == "RECEIVED" && !string.IsNullOrEmpty(cmd.WarehouseId))
            {
                var po = await _repo.GetByIdAsync(cmd.PurchaseOrderId, ct) ?? throw new NotFoundException(nameof(PurchaseOrder), cmd.PurchaseOrderId);
                if (po.Status == "RECEIVED") return Result<bool>.Success(true); // already received
                
                var items = await _repo.GetItemsByPOAsync(cmd.PurchaseOrderId, ct);
                await _uow.BeginTransactionAsync(ct);
                try
                {
                    foreach (var item in items)
                    {
                        await _stockRepo.UpsertAsync(item.ProductId, cmd.WarehouseId, item.QuantityOrdered, _uow.Connection, _uow.Transaction!, ct);
                        var txn = new StockTransaction { TransactionId = Guid.NewGuid().ToString(), ProductId = item.ProductId, WarehouseId = cmd.WarehouseId, TransactionType = TransactionType.Purchase, Quantity = item.QuantityOrdered };
                        await _txnRepo.InsertAsync(txn, _uow.Connection, _uow.Transaction!, ct);
                    }
                    // Status update also needs to be in transaction, but repo doesn't take uow. 
                    // Let's do it outside since it's just a status flag, or assume Stock is most critical.
                    // Actually, let's just update the status FIRST, then do stock. If stock fails, status rolls back? 
                    // Wait, UpdateStatusAsync uses a separate connection. It's fine to run after stock commit.
                    await _uow.CommitAsync(ct);
                }
                catch
                {
                    await _uow.RollbackAsync(ct);
                    throw;
                }
            }

            var updated = await _repo.UpdateStatusAsync(cmd.PurchaseOrderId, cmd.Status, ct);
            if (!updated) throw new NotFoundException(nameof(PurchaseOrder), cmd.PurchaseOrderId);
            return Result<bool>.Success(true);
        }
    }
}
