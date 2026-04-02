// Features/Reports/Queries/GetInventoryTurnover/GetInventoryTurnoverQuery.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Reports.Queries.GetInventoryTurnover
{
    /// <summary>
    /// Inventory Turnover Analysis.
    /// Formula: Turnover Ratio = Total Units Sold / Average Qty On Hand (for the period)
    /// Days in Inventory = Period Days / Turnover Ratio
    /// Higher turnover = faster-moving stock (better efficiency).
    /// </summary>
    /// <param name="FromDate">Start of analysis period.</param>
    /// <param name="ToDate">End of analysis period.</param>
    public record GetInventoryTurnoverQuery(
        DateTime FromDate,
        DateTime ToDate
    ) : IRequest<Result<List<TurnoverItem>>>;

    public class TurnoverItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalUnitsSold { get; set; }
        public decimal AverageQtyOnHand { get; set; }

        /// <summary>Turnover Ratio: how many times inventory was replenished in the period.</summary>
        public decimal TurnoverRatio =>
            AverageQtyOnHand == 0 ? 0 : Math.Round(TotalUnitsSold / AverageQtyOnHand, 2);

        public int PeriodDays { get; set; }

        /// <summary>Days sales of inventory: how many days worth of stock is on hand.</summary>
        public decimal DaysInInventory =>
            TurnoverRatio == 0 ? PeriodDays : Math.Round(PeriodDays / TurnoverRatio, 1);

        /// <summary>Classification based on turnover speed.</summary>
        public string Classification => TurnoverRatio switch
        {
            > 12 => "Fast Moving",
            > 4  => "Normal",
            > 1  => "Slow Moving",
            _    => "Dead Stock"
        };
    }
}
