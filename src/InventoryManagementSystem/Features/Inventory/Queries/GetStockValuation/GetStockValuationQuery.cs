// Features/Inventory/Queries/GetStockValuation/GetStockValuationQuery.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Inventory.Queries.GetStockValuation
{
    /// <summary>
    /// Query to calculate the inventory value for a given product/warehouse
    /// using the selected valuation method (FIFO, LIFO, or WeightedAverage).
    /// </summary>
    /// <param name="ProductId">Required. The product to value.</param>
    /// <param name="WarehouseId">Optional. If null, values across all warehouses.</param>
    /// <param name="ValuationMethod">FIFO | LIFO | WeightedAverage. Defaults to appsettings.</param>
    public record GetStockValuationQuery(
        string ProductId,
        string? WarehouseId,
        string ValuationMethod = "FIFO"
    ) : IRequest<Result<StockValuationResult>>;
}
