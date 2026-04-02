// Features/Reports/Queries/GetStockAgingReport/GetStockAgingReportQuery.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Reports.Queries.GetStockAgingReport
{
    /// <summary>
    /// Stock Aging Report — groups stock transactions by age buckets:
    ///   0–30 days, 31–60 days, 61–90 days, 90+ days.
    /// Helps identify slow-moving and obsolete inventory.
    /// </summary>
    public record GetStockAgingReportQuery : IRequest<Result<StockAgingResult>>;

    public class StockAgingResult
    {
        public List<AgingBucket> Buckets { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class AgingBucket
    {
        public string BucketLabel { get; set; } = string.Empty;   // e.g. "0-30 Days"
        public int MinDays { get; set; }
        public int MaxDays { get; set; }                           // -1 = no upper bound
        public List<AgingProduct> Products { get; set; } = new();
        public decimal TotalQuantity => Products.Sum(p => p.Quantity);
        public decimal TotalValue    => Products.Sum(p => p.EstimatedValue);
    }

    public class AgingProduct
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public int DaysSinceLastMovement { get; set; }
        public decimal UnitCost { get; set; }
        public decimal EstimatedValue => Quantity * UnitCost;
    }
}
