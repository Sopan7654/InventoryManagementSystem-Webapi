// ============================================================
// FILE: src/InventoryManagement.Application/Features/Inventory/Queries/InventoryQueries.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Inventory.Queries
{
    /// <summary>Query to get current stock levels with pagination.</summary>
    public record GetStockLevelQuery(PaginationParams Pagination, string? ProductId = null, string? WarehouseId = null)
        : IRequest<Result<PaginatedResponse<StockLevelResponseDto>>>;

    /// <summary>Query to get low stock report.</summary>
    public record GetLowStockReportQuery() : IRequest<Result<IEnumerable<StockLevelResponseDto>>>, ICacheable
    {
        public string CacheKey => Constants.CacheKeys.LowStockReport;
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
    }

    /// <summary>Query to get stock transaction history.</summary>
    public record GetStockTransactionHistoryQuery(PaginationParams Pagination, string? ProductId = null)
        : IRequest<Result<PaginatedResponse<StockTransactionResponseDto>>>;
}
