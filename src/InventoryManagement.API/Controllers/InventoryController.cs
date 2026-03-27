// ============================================================
// FILE: src/InventoryManagement.API/Controllers/InventoryController.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Inventory.Commands;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Authorize]
    public class InventoryController : BaseApiController
    {
        /// <summary>Get stock levels with optional filters.</summary>
        [HttpGet("stock-levels")]
        public async Task<IActionResult> GetStockLevels([FromQuery] PaginationParams pagination,
            [FromQuery] string? productId = null, [FromQuery] string? warehouseId = null)
            => FromResult(await Mediator.Send(new GetStockLevelQuery(pagination, productId, warehouseId)));

        /// <summary>Get low stock report.</summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
            => FromResult(await Mediator.Send(new GetLowStockReportQuery()));

        /// <summary>Get transaction history.</summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] PaginationParams pagination,
            [FromQuery] string? productId = null)
            => FromResult(await Mediator.Send(new GetStockTransactionHistoryQuery(pagination, productId)));

        /// <summary>Stock In — receive inventory.</summary>
        [HttpPost("stock-in")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<IActionResult> StockIn([FromBody] StockInDto dto)
            => FromResult(await Mediator.Send(new StockInCommand(dto)));

        /// <summary>Stock Out — ship inventory.</summary>
        [HttpPost("stock-out")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<IActionResult> StockOut([FromBody] StockOutDto dto)
            => FromResult(await Mediator.Send(new StockOutCommand(dto)));

        /// <summary>Transfer stock between warehouses.</summary>
        [HttpPost("transfer")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<IActionResult> Transfer([FromBody] StockTransferDto dto)
            => FromResult(await Mediator.Send(new TransferStockCommand(dto)));

        /// <summary>Place stock on hold.</summary>
        [HttpPost("hold")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<IActionResult> Hold([FromBody] HoldStockDto dto)
            => FromResult(await Mediator.Send(new HoldStockCommand(dto)));

        /// <summary>Adjust stock levels.</summary>
        [HttpPost("adjustment")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Adjust([FromBody] AdjustmentDto dto)
            => FromResult(await Mediator.Send(new AdjustmentCommand(dto)));
    }
}
