// ============================================================
// FILE: src/InventoryManagement.Application/Features/Inventory/Commands/InventoryCommandHandlers.cs
// ============================================================
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Events;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Inventory.Commands
{
    /// <summary>Handles StockInCommand — migrated from InventoryService.StockIn.</summary>
    public class StockInCommandHandler : IRequestHandler<StockInCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockInCommandHandler> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;

        public StockInCommandHandler(IUnitOfWork unitOfWork, ILogger<StockInCommandHandler> logger,
            ICacheService cacheService, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<string>> Handle(StockInCommand request, CancellationToken cancellationToken)
        {
            string productId = request.Dto.ProductId;
            string warehouseId = request.Dto.WarehouseId;
            decimal qty = request.Dto.Quantity;

            Product? product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return Result<string>.NotFound($"Product '{productId}' does not exist.");

            Warehouse? warehouse = await _unitOfWork.Warehouses.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null) return Result<string>.NotFound($"Warehouse '{warehouseId}' does not exist.");

            StockLevel? stock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
            decimal previousQty = stock?.QuantityOnHand ?? 0;

            if (stock != null)
            {
                stock.QuantityOnHand += qty;
                _unitOfWork.StockLevels.Update(stock);
            }
            else
            {
                stock = new StockLevel
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    QuantityOnHand = qty,
                    ReorderLevel = 0,
                    SafetyStock = 0,
                    ReservedQuantity = 0
                };
                await _unitOfWork.StockLevels.AddAsync(stock, cancellationToken);
            }

            StockTransaction transaction = new()
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                TransactionType = TransactionType.PURCHASE,
                Quantity = qty,
                Reference = request.Dto.Reference
            };
            await _unitOfWork.StockTransactions.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, warehouseId), cancellationToken);

            await _eventPublisher.PublishAsync(new StockLevelChangedEvent(
                productId, warehouseId, previousQty, stock.QuantityOnHand, stock.ReorderLevel), cancellationToken);

            _logger.LogInformation("Stock In: +{Qty} units for product {ProductId} at warehouse {WarehouseId}", qty, productId, warehouseId);
            return Result<string>.Success($"Stock In successful. +{qty} units added.");
        }
    }

    /// <summary>Handles StockOutCommand — migrated from InventoryService.StockOut.</summary>
    public class StockOutCommandHandler : IRequestHandler<StockOutCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockOutCommandHandler> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;

        public StockOutCommandHandler(IUnitOfWork unitOfWork, ILogger<StockOutCommandHandler> logger,
            ICacheService cacheService, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<string>> Handle(StockOutCommand request, CancellationToken cancellationToken)
        {
            string productId = request.Dto.ProductId;
            string warehouseId = request.Dto.WarehouseId;
            decimal qty = request.Dto.Quantity;

            Product? product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return Result<string>.NotFound($"Product '{productId}' does not exist.");

            Warehouse? warehouse = await _unitOfWork.Warehouses.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null) return Result<string>.NotFound($"Warehouse '{warehouseId}' does not exist.");

            StockLevel? stock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
            if (stock == null) return Result<string>.ValidationError("No stock record found for this product/warehouse combination.");
            if (stock.AvailableQuantity < qty)
                return Result<string>.ValidationError($"Insufficient stock. Available: {stock.AvailableQuantity:N2}, Requested: {qty:N2}");

            decimal previousQty = stock.QuantityOnHand;
            stock.QuantityOnHand -= qty;
            _unitOfWork.StockLevels.Update(stock);

            StockTransaction transaction = new()
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                TransactionType = TransactionType.SALE,
                Quantity = qty,
                Reference = request.Dto.Reference
            };
            await _unitOfWork.StockTransactions.AddAsync(transaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, warehouseId), cancellationToken);

            await _eventPublisher.PublishAsync(new StockLevelChangedEvent(
                productId, warehouseId, previousQty, stock.QuantityOnHand, stock.ReorderLevel), cancellationToken);

            _logger.LogInformation("Stock Out: -{Qty} units for product {ProductId} at warehouse {WarehouseId}", qty, productId, warehouseId);
            return Result<string>.Success($"Stock Out successful. -{qty} units removed.");
        }
    }

    /// <summary>Handles TransferStockCommand — migrated from InventoryService.Transfer.</summary>
    public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransferStockCommandHandler> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;

        public TransferStockCommandHandler(IUnitOfWork unitOfWork, ILogger<TransferStockCommandHandler> logger,
            ICacheService cacheService, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<string>> Handle(TransferStockCommand request, CancellationToken cancellationToken)
        {
            string productId = request.Dto.ProductId;
            string fromId = request.Dto.FromWarehouseId;
            string toId = request.Dto.ToWarehouseId;
            decimal qty = request.Dto.Quantity;

            Product? product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return Result<string>.NotFound($"Product '{productId}' does not exist.");

            Warehouse? fromWh = await _unitOfWork.Warehouses.GetByIdAsync(fromId, cancellationToken);
            if (fromWh == null) return Result<string>.NotFound($"Source warehouse '{fromId}' does not exist.");

            Warehouse? toWh = await _unitOfWork.Warehouses.GetByIdAsync(toId, cancellationToken);
            if (toWh == null) return Result<string>.NotFound($"Destination warehouse '{toId}' does not exist.");

            StockLevel? sourceStock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, fromId, cancellationToken);
            if (sourceStock == null) return Result<string>.ValidationError("No stock in source warehouse for this product.");
            if (sourceStock.AvailableQuantity < qty)
                return Result<string>.ValidationError($"Insufficient stock in source. Available: {sourceStock.AvailableQuantity:N2}");

            string reference = $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Deduct from source
            sourceStock.QuantityOnHand -= qty;
            _unitOfWork.StockLevels.Update(sourceStock);

            // Add to destination (create if first time)
            StockLevel? destStock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, toId, cancellationToken);
            if (destStock != null)
            {
                destStock.QuantityOnHand += qty;
                _unitOfWork.StockLevels.Update(destStock);
            }
            else
            {
                destStock = new StockLevel
                {
                    ProductId = productId,
                    WarehouseId = toId,
                    QuantityOnHand = qty
                };
                await _unitOfWork.StockLevels.AddAsync(destStock, cancellationToken);
            }

            // Log both sides
            await _unitOfWork.StockTransactions.AddAsync(new StockTransaction
            {
                ProductId = productId, WarehouseId = fromId,
                TransactionType = TransactionType.TRANSFER_OUT, Quantity = qty, Reference = reference
            }, cancellationToken);

            await _unitOfWork.StockTransactions.AddAsync(new StockTransaction
            {
                ProductId = productId, WarehouseId = toId,
                TransactionType = TransactionType.TRANSFER_IN, Quantity = qty, Reference = reference
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, fromId), cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, toId), cancellationToken);

            _logger.LogInformation("Transfer: {Qty} units of {ProductId} from {From} to {To}. Ref: {Ref}",
                qty, productId, fromId, toId, reference);
            return Result<string>.Success($"Transfer complete. {qty} units moved. Ref: {reference}");
        }
    }

    /// <summary>Handles HoldStockCommand — migrated from InventoryService.HoldStock.</summary>
    public class HoldStockCommandHandler : IRequestHandler<HoldStockCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HoldStockCommandHandler> _logger;
        private readonly ICacheService _cacheService;

        public HoldStockCommandHandler(IUnitOfWork unitOfWork, ILogger<HoldStockCommandHandler> logger, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<Result<string>> Handle(HoldStockCommand request, CancellationToken cancellationToken)
        {
            string productId = request.Dto.ProductId;
            string warehouseId = request.Dto.WarehouseId;
            decimal qty = request.Dto.Quantity;

            Product? product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return Result<string>.NotFound($"Product '{productId}' does not exist.");

            Warehouse? warehouse = await _unitOfWork.Warehouses.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null) return Result<string>.NotFound($"Warehouse '{warehouseId}' does not exist.");

            StockLevel? stock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
            if (stock == null) return Result<string>.ValidationError("No stock record found for this product/warehouse combination.");
            if (stock.AvailableQuantity < qty)
                return Result<string>.ValidationError($"Cannot hold {qty:N2} units. Available (unreserved): {stock.AvailableQuantity:N2}");

            stock.ReservedQuantity += qty;
            _unitOfWork.StockLevels.Update(stock);

            await _unitOfWork.StockTransactions.AddAsync(new StockTransaction
            {
                ProductId = productId, WarehouseId = warehouseId,
                TransactionType = TransactionType.HOLD, Quantity = qty
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, warehouseId), cancellationToken);

            _logger.LogInformation("Hold: {Qty} units reserved for product {ProductId} at warehouse {WarehouseId}", qty, productId, warehouseId);
            return Result<string>.Success($"{qty} units reserved/held successfully.");
        }
    }

    /// <summary>Handles AdjustmentCommand — migrated from InventoryService.Adjustment.</summary>
    public class AdjustmentCommandHandler : IRequestHandler<AdjustmentCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdjustmentCommandHandler> _logger;
        private readonly ICacheService _cacheService;
        private readonly IEventPublisher _eventPublisher;

        public AdjustmentCommandHandler(IUnitOfWork unitOfWork, ILogger<AdjustmentCommandHandler> logger,
            ICacheService cacheService, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Result<string>> Handle(AdjustmentCommand request, CancellationToken cancellationToken)
        {
            string productId = request.Dto.ProductId;
            string warehouseId = request.Dto.WarehouseId;
            decimal qty = request.Dto.Quantity;

            Product? product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null) return Result<string>.NotFound($"Product '{productId}' does not exist.");

            Warehouse? warehouse = await _unitOfWork.Warehouses.GetByIdAsync(warehouseId, cancellationToken);
            if (warehouse == null) return Result<string>.NotFound($"Warehouse '{warehouseId}' does not exist.");

            StockLevel? stock = await _unitOfWork.StockLevels.GetByProductAndWarehouseAsync(productId, warehouseId, cancellationToken);
            if (stock == null) return Result<string>.ValidationError("No stock record found for this product/warehouse combination.");

            decimal projected = stock.QuantityOnHand + qty;
            if (projected < 0)
                return Result<string>.ValidationError($"Adjustment would result in negative stock ({projected:N2}). Current on-hand: {stock.QuantityOnHand:N2}");

            decimal previousQty = stock.QuantityOnHand;
            stock.QuantityOnHand = projected;
            _unitOfWork.StockLevels.Update(stock);

            await _unitOfWork.StockTransactions.AddAsync(new StockTransaction
            {
                ProductId = productId, WarehouseId = warehouseId,
                TransactionType = TransactionType.ADJUSTMENT, Quantity = qty,
                Reference = request.Dto.Reason
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync(string.Format(Constants.CacheKeys.StockLevel, productId, warehouseId), cancellationToken);

            await _eventPublisher.PublishAsync(new StockLevelChangedEvent(
                productId, warehouseId, previousQty, stock.QuantityOnHand, stock.ReorderLevel), cancellationToken);

            string sign = qty >= 0 ? "+" : "";
            _logger.LogInformation("Adjustment: {Sign}{Qty} units for product {ProductId}", sign, qty, productId);
            return Result<string>.Success($"Adjustment applied: {sign}{qty} units. New on-hand: {projected:N2}");
        }
    }
}
