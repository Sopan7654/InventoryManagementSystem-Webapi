// Features/Inventory/Queries/GetStockValuation/StockValuationResult.cs
namespace InventoryManagementSystem.Features.Inventory.Queries.GetStockValuation
{
    /// <summary>DTO returned by the stock valuation query.</summary>
    public class StockValuationResult
    {
        public string ProductId { get; set; } = string.Empty;
        public string? WarehouseId { get; set; }
        public string ValuationMethod { get; set; } = string.Empty;

        /// <summary>Total quantity currently on hand.</summary>
        public decimal QuantityOnHand { get; set; }

        /// <summary>Total calculated inventory value using the chosen method.</summary>
        public decimal TotalValue { get; set; }

        /// <summary>Average cost per unit under the chosen method.</summary>
        public decimal AverageCostPerUnit => QuantityOnHand == 0 ? 0 : TotalValue / QuantityOnHand;

        /// <summary>Per-layer breakdown (each Inbound layer that contributes to value).</summary>
        public List<ValuationLayer> Layers { get; set; } = new();

        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>One cost layer — used in FIFO/LIFO breakdown.</summary>
    public class ValuationLayer
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal QuantityIn { get; set; }
        public decimal QuantityUsed { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LayerValue => QuantityUsed * UnitCost;
    }
}
