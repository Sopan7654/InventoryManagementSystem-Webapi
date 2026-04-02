// Features/Reports/Queries/GetStockAgingReport/GetStockAgingReportQueryHandler.cs
// Stock aging groups products by days since last inbound movement to identify
// slow-moving or obsolete inventory — important for warehouse cost control.
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Inventory.Repository;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Reports.Queries.GetStockAgingReport
{
    public sealed class GetStockAgingReportQueryHandler
        : IRequestHandler<GetStockAgingReportQuery, Result<StockAgingResult>>
    {
        private readonly IStockLevelRepository      _stockRepo;
        private readonly IStockTransactionRepository _txnRepo;
        private readonly IProductRepository          _productRepo;

        public GetStockAgingReportQueryHandler(
            IStockLevelRepository stockRepo,
            IStockTransactionRepository txnRepo,
            IProductRepository productRepo)
        {
            _stockRepo   = stockRepo;
            _txnRepo     = txnRepo;
            _productRepo = productRepo;
        }

        public async Task<Result<StockAgingResult>> Handle(
            GetStockAgingReportQuery query, CancellationToken ct)
        {
            var stockLevels = await _stockRepo.GetAllAsync(ct);
            var products    = await _productRepo.GetAllAsync(ct);
            var costMap     = products.ToDictionary(p => p.ProductId, p => p.Cost);

            // Build aging items: find last transaction date per product+warehouse
            var agingItems = new List<AgingProduct>();
            var today      = DateTime.Today;

            foreach (var sl in stockLevels.Where(s => s.QuantityOnHand > 0))
            {
                // Get last inbound transaction for this product
                var txns = await _txnRepo.GetByProductAsync(sl.ProductId, ct);
                var lastTxn = txns.OrderByDescending(t => t.TransactionDate).FirstOrDefault();

                var lastDate = lastTxn?.TransactionDate.Date ?? today;
                var daysSince = (today - lastDate).Days;

                agingItems.Add(new AgingProduct
                {
                    ProductId            = sl.ProductId,
                    ProductName          = sl.ProductName,
                    WarehouseName        = sl.WarehouseName,
                    Quantity             = sl.QuantityOnHand,
                    LastTransactionDate  = lastDate,
                    DaysSinceLastMovement = daysSince,
                    UnitCost             = costMap.GetValueOrDefault(sl.ProductId, 0m)
                });
            }

            // Define age buckets
            var buckets = new[]
            {
                new AgingBucket { BucketLabel = "0–30 Days",  MinDays = 0,  MaxDays = 30  },
                new AgingBucket { BucketLabel = "31–60 Days", MinDays = 31, MaxDays = 60  },
                new AgingBucket { BucketLabel = "61–90 Days", MinDays = 61, MaxDays = 90  },
                new AgingBucket { BucketLabel = "90+ Days",   MinDays = 91, MaxDays = -1  }
            };

            // Assign products to buckets
            foreach (var item in agingItems)
            {
                var bucket = buckets.FirstOrDefault(b =>
                    item.DaysSinceLastMovement >= b.MinDays &&
                    (b.MaxDays == -1 || item.DaysSinceLastMovement <= b.MaxDays))
                    ?? buckets[^1];

                bucket.Products.Add(item);
            }

            return Result<StockAgingResult>.Success(new StockAgingResult
            {
                Buckets = buckets.ToList()
            });
        }
    }
}
