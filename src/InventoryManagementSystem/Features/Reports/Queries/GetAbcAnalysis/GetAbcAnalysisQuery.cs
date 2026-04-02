// Features/Reports/Queries/GetAbcAnalysis/GetAbcAnalysisQuery.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Reports.Queries.GetAbcAnalysis
{
    /// <summary>
    /// ABC Analysis — classifies products by inventory value (Cost × QtyOnHand):
    ///   A = top 20% cumulative value (high-value, tight control)
    ///   B = next 30% cumulative value (medium control)
    ///   C = remaining 50% (low value, minimal control)
    /// Based on Pareto Principle, as required by the project spec.
    /// </summary>
    public record GetAbcAnalysisQuery : IRequest<Result<AbcAnalysisResult>>;

    public class AbcAnalysisResult
    {
        public List<AbcProduct> ClassA { get; set; } = new();   // top 20% value
        public List<AbcProduct> ClassB { get; set; } = new();   // next 30% value
        public List<AbcProduct> ClassC { get; set; } = new();   // remaining 50%
        public decimal TotalInventoryValue { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class AbcProduct
    {
        public string ProductId { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
        public decimal CumulativeValuePercent { get; set; }
        public string AbcClass { get; set; } = string.Empty;  // "A", "B", or "C"
    }
}
