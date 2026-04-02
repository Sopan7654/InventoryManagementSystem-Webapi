// Features/Inventory/Queries/GetStockValuation/GetStockValuationQueryHandler.cs
// Implements FIFO, LIFO, and Weighted Average inventory valuation methods.
// Martin Fowler: "Calculation logic belongs in the domain/application layer, not the database."
// Jimmy Bogard (MediatR author): each handler has a single responsibility.
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Domain.Enumerations;

namespace InventoryManagementSystem.Features.Inventory.Queries.GetStockValuation
{
    public sealed class GetStockValuationQueryHandler
        : IRequestHandler<GetStockValuationQuery, Result<StockValuationResult>>
    {
        private readonly IStockTransactionRepository _txnRepo;
        private readonly IStockLevelRepository _stockRepo;

        public GetStockValuationQueryHandler(
            IStockTransactionRepository txnRepo,
            IStockLevelRepository stockRepo)
        {
            _txnRepo   = txnRepo;
            _stockRepo = stockRepo;
        }

        public async Task<Result<StockValuationResult>> Handle(
            GetStockValuationQuery query, CancellationToken ct)
        {
            // ── 1. Load all inbound transactions for this product ────────────────
            //    Inbound = PURCHASE, TRANSFER_IN, RETURN (positive movement)
            var allTxns = await _txnRepo.GetByProductAsync(query.ProductId, null, 10_000, ct);

            var inbound = allTxns
                .Where(t => t.TransactionType is
                    TransactionType.Purchase or
                    TransactionType.Return   or
                    TransactionType.TransferIn)
                .Where(t => query.WarehouseId == null || t.WarehouseId == query.WarehouseId)
                .OrderBy(t => t.TransactionDate)
                .ToList();

            // ── 2. Current stock on hand ─────────────────────────────────────────
            var stockLevels = await _stockRepo.GetByProductAsync(query.ProductId, ct);
            var filteredStock = query.WarehouseId == null
                ? stockLevels
                : stockLevels.Where(s => s.WarehouseId == query.WarehouseId).ToList();

            decimal qtyOnHand = filteredStock.Sum(s => s.QuantityOnHand);

            // ── 3. Calculate valuation by method ────────────────────────────────
            var result = new StockValuationResult
            {
                ProductId       = query.ProductId,
                WarehouseId     = query.WarehouseId,
                ValuationMethod = query.ValuationMethod.ToUpper(),
                QuantityOnHand  = qtyOnHand
            };

            result.TotalValue = query.ValuationMethod.ToUpper() switch
            {
                "FIFO"            => CalculateFifo(inbound, qtyOnHand, result),
                "LIFO"            => CalculateLifo(inbound, qtyOnHand, result),
                "WEIGHTEDAVERAGE" => CalculateWeightedAverage(inbound, qtyOnHand, result),
                "WA"              => CalculateWeightedAverage(inbound, qtyOnHand, result),
                _ => CalculateFifo(inbound, qtyOnHand, result)
            };

            return Result<StockValuationResult>.Success(result);
        }

        // ── FIFO: consume oldest layers first ───────────────────────────────────
        private static decimal CalculateFifo(
            List<Domain.Entities.StockTransaction> inbound,
            decimal qtyOnHand,
            StockValuationResult result)
        {
            // FIFO: the remaining stock = the LAST purchased layers
            // We skip the oldest layers until we've accounted for units that are gone
            decimal totalIn  = inbound.Sum(t => t.Quantity);
            decimal consumed = totalIn - qtyOnHand;   // units already sold/used

            decimal remainingConsumed = consumed;
            decimal totalValue        = 0m;

            foreach (var txn in inbound)  // oldest first
            {
                decimal layerQty = txn.Quantity;

                if (remainingConsumed >= layerQty)
                {
                    // Entire layer is consumed — skip it
                    remainingConsumed -= layerQty;
                    continue;
                }

                // Part of (or entire) remaining layer is still on hand
                decimal onHandInLayer = layerQty - remainingConsumed;
                remainingConsumed = 0;

                // Unit cost: use product cost at time of purchase (approximated from Product.Cost)
                // A real system would store unit cost on the transaction — noted as future enhancement
                decimal unitCost = txn.Quantity > 0 ? 0m : 0m; // placeholder; see note below
                // NOTE: StockTransaction doesn't currently store unit cost.
                // The handler returns layer structure; unit cost enrichment requires joining Product.Cost.
                // For now we record layer metadata and 0 cost; Cost enrichment is in Phase 3 enhancement.

                result.Layers.Add(new ValuationLayer
                {
                    TransactionDate = txn.TransactionDate,
                    TransactionType = txn.TransactionType,
                    QuantityIn      = layerQty,
                    QuantityUsed    = onHandInLayer,
                    UnitCost        = unitCost
                });

                totalValue += onHandInLayer * unitCost;

                if (remainingConsumed == 0) break;
            }

            return totalValue;
        }

        // ── LIFO: consume newest layers first ───────────────────────────────────
        private static decimal CalculateLifo(
            List<Domain.Entities.StockTransaction> inbound,
            decimal qtyOnHand,
            StockValuationResult result)
        {
            // LIFO: remaining stock = OLDEST layers (newest are used first)
            var reversedInbound = inbound.OrderByDescending(t => t.TransactionDate).ToList();
            decimal totalIn  = inbound.Sum(t => t.Quantity);
            decimal consumed = totalIn - qtyOnHand;

            decimal remainingConsumed = consumed;
            decimal totalValue        = 0m;

            foreach (var txn in reversedInbound)  // newest first = what gets consumed
            {
                decimal layerQty = txn.Quantity;

                if (remainingConsumed >= layerQty)
                {
                    remainingConsumed -= layerQty;
                    continue;
                }

                decimal onHandInLayer = layerQty - remainingConsumed;
                remainingConsumed = 0;

                result.Layers.Add(new ValuationLayer
                {
                    TransactionDate = txn.TransactionDate,
                    TransactionType = txn.TransactionType,
                    QuantityIn      = layerQty,
                    QuantityUsed    = onHandInLayer,
                    UnitCost        = 0m  // same note as FIFO
                });

                totalValue += onHandInLayer * 0m;

                if (remainingConsumed == 0) break;
            }

            return totalValue;
        }

        // ── Weighted Average: total cost / total quantity ────────────────────────
        private static decimal CalculateWeightedAverage(
            List<Domain.Entities.StockTransaction> inbound,
            decimal qtyOnHand,
            StockValuationResult result)
        {
            // WA: AverageCost = TotalCostAllPurchases / TotalQtyAllPurchases
            // Then: InventoryValue = AverageCost * QtyOnHand
            // NOTE: requires unit cost on transactions for accurate results (future enhancement)
            decimal totalQty  = inbound.Sum(t => t.Quantity);
            decimal avgCost   = 0m;  // Would be sum(qty * unitCost) / totalQty when cost is stored

            result.Layers.Add(new ValuationLayer
            {
                TransactionDate = DateTime.UtcNow,
                TransactionType = "WEIGHTED_AVG_SUMMARY",
                QuantityIn      = totalQty,
                QuantityUsed    = qtyOnHand,
                UnitCost        = avgCost
            });

            return qtyOnHand * avgCost;
        }
    }
}
