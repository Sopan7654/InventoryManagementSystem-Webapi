// Features/Reports/Queries/GetInventoryTurnover/GetInventoryTurnoverQueryHandler.cs
// Inventory Turnover = Units Sold in Period / Average Qty On Hand.
// Classified by speed: Fast Moving / Normal / Slow Moving / Dead Stock.
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Enumerations;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Reports.Queries.GetInventoryTurnover
{
    public sealed class GetInventoryTurnoverQueryHandler
        : IRequestHandler<GetInventoryTurnoverQuery, Result<List<TurnoverItem>>>
    {
        private readonly IStockTransactionRepository _txnRepo;
        private readonly IStockLevelRepository       _stockRepo;
        private readonly IProductRepository          _productRepo;

        public GetInventoryTurnoverQueryHandler(
            IStockTransactionRepository txnRepo,
            IStockLevelRepository stockRepo,
            IProductRepository productRepo)
        {
            _txnRepo     = txnRepo;
            _stockRepo   = stockRepo;
            _productRepo = productRepo;
        }

        public async Task<Result<List<TurnoverItem>>> Handle(
            GetInventoryTurnoverQuery query, CancellationToken ct)
        {
            var products    = await _productRepo.GetAllAsync(ct);
            var stockLevels = await _stockRepo.GetAllAsync(ct);
            var allTxns     = await _txnRepo.GetAllAsync(limit: 100_000, ct: ct);

            int periodDays = Math.Max(1, (query.ToDate - query.FromDate).Days);

            // Filter only SALE / TRANSFER_OUT transactions in the period
            var outboundTxns = allTxns
                .Where(t => t.TransactionDate >= query.FromDate &&
                            t.TransactionDate <= query.ToDate &&
                            (t.TransactionType == TransactionType.Sale ||
                             t.TransactionType == TransactionType.TransferOut))
                .GroupBy(t => t.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Quantity));

            // Average qty on hand per product (sum across warehouses)
            var avgQtyMap = stockLevels
                .GroupBy(s => s.ProductId)
                .ToDictionary(g => g.Key, g => g.Average(s => s.QuantityOnHand));

            var result = products
                .Where(p => p.IsActive)
                .Select(p => new TurnoverItem
                {
                    ProductId       = p.ProductId,
                    SKU             = p.SKU,
                    ProductName     = p.ProductName,
                    TotalUnitsSold  = outboundTxns.GetValueOrDefault(p.ProductId, 0m),
                    AverageQtyOnHand = avgQtyMap.GetValueOrDefault(p.ProductId, 0m),
                    PeriodDays      = periodDays
                })
                .OrderByDescending(t => t.TurnoverRatio)
                .ToList();

            return Result<List<TurnoverItem>>.Success(result);
        }
    }
}
