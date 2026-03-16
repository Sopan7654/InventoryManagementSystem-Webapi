// Features/Reports/ReportsController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Reports.Queries.GetLowStockReport;
using InventoryManagementSystem.Features.Reports.Queries.GetTransactionHistory;
using InventoryManagementSystem.Features.Batches.Queries.GetExpiringBatches;

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
    }
}
