// Features/Reports/ReportsController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Reports.Queries.GetLowStockReport;
using InventoryManagementSystem.Features.Reports.Queries.GetTransactionHistory;
using InventoryManagementSystem.Features.Batches.Queries.GetExpiringBatches;
using InventoryManagementSystem.Features.Reports.Queries.GetOverstockReport;
using InventoryManagementSystem.Features.Reports.Queries.GetAbcAnalysis;
using InventoryManagementSystem.Features.Reports.Queries.GetStockAgingReport;
using InventoryManagementSystem.Features.Reports.Queries.GetInventoryTurnover;

namespace InventoryManagementSystem.Features.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator) => _mediator = mediator;

        /// <summary>Products whose stock is at or below re-order level.</summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetLowStockReportQuery(), ct);
            return Ok(new { success = true, count = result.Value!.Count, data = result.Value });
        }

        /// <summary>Batches expiring within the specified days window.</summary>
        [HttpGet("expiring-batches")]
        public async Task<IActionResult> ExpiringBatches([FromQuery] int days = 30, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetExpiringBatchesQuery(days), ct);
            return Ok(new { success = true, count = result.Value!.Count, data = result.Value });
        }

        /// <summary>Full stock transaction history (optionally filtered by product).</summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> Transactions(
            [FromQuery] string? productId = null,
            [FromQuery] int limit = 50,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetTransactionHistoryQuery(productId, limit), ct);
            return Ok(new { success = true, count = result.Value!.Count, data = result.Value });
        }

        /// <summary>
        /// Overstock alert — products with quantity on hand exceeding threshold × reorder level.
        /// Default threshold multiplier is 3 (3x reorder level = overstocked).
        /// </summary>
        [HttpGet("overstock")]
        public async Task<IActionResult> Overstock(
            [FromQuery] decimal threshold = 3m,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetOverstockReportQuery(threshold), ct);
            return Ok(new { success = true, count = result.Value!.Count, data = result.Value });
        }

        /// <summary>
        /// ABC Analysis — classifies all active products into A, B, C segments by inventory value.
        /// A = top 80% of value, B = next 15%, C = bottom 5%.
        /// </summary>
        [HttpGet("abc-analysis")]
        public async Task<IActionResult> AbcAnalysis(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAbcAnalysisQuery(), ct);
            return Ok(new
            {
                success              = true,
                totalInventoryValue  = result.Value!.TotalInventoryValue,
                classACount          = result.Value.ClassA.Count,
                classBCount          = result.Value.ClassB.Count,
                classCCount          = result.Value.ClassC.Count,
                data                 = result.Value
            });
        }

        /// <summary>
        /// Stock Aging Report — groups stock by days since last movement:
        /// 0–30 days, 31–60 days, 61–90 days, 90+ days.
        /// </summary>
        [HttpGet("stock-aging")]
        public async Task<IActionResult> StockAging(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetStockAgingReportQuery(), ct);
            return Ok(new { success = true, data = result.Value });
        }

        /// <summary>
        /// Inventory Turnover Analysis for a given date range.
        /// Returns turnover ratio and days-in-inventory per product.
        /// </summary>
        [HttpGet("turnover")]
        public async Task<IActionResult> Turnover(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetInventoryTurnoverQuery(from, to), ct);
            return Ok(new { success = true, count = result.Value!.Count, data = result.Value });
        }
    }
}
