// Features/Reports/Queries/GetOverstockReport/GetOverstockReportQuery.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Reports.Queries.GetOverstockReport
{
    /// <summary>
    /// Returns products where QuantityOnHand exceeds the configured overstock threshold
    /// (default: 3x the ReorderLevel). Supports the Overstock Alert deliverable.
    /// </summary>
    /// <param name="ThresholdMultiplier">Products with QtyOnHand > ReorderLevel × threshold are considered overstocked. Default 3.</param>
    public record GetOverstockReportQuery(decimal ThresholdMultiplier = 3m)
        : IRequest<Result<List<OverstockItem>>>;

    public class OverstockItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal OverstockThreshold { get; set; }
        public decimal ExcessQuantity => QuantityOnHand - OverstockThreshold;
    }
}
