// Features/Inventory/InventoryController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Inventory.Commands.Adjustment;
using InventoryManagementSystem.Features.Inventory.Commands.HoldStock;
using InventoryManagementSystem.Features.Inventory.Commands.StockIn;
using InventoryManagementSystem.Features.Inventory.Commands.StockOut;
using InventoryManagementSystem.Features.Inventory.Commands.Transfer;
using InventoryManagementSystem.Features.Inventory.Queries.GetStockLevels;
namespace InventoryManagementSystem.Features.Inventory
{
    [ApiController][Route("api/[controller]")][Produces("application/json")]
    public sealed class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InventoryController(IMediator mediator) => _mediator = mediator;

        /// <summary>Get all current stock levels.</summary>
        [HttpGet("stock-levels")]
        public async Task<IActionResult> GetStockLevels(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetStockLevelsQuery(), ct)).Value });

        /// <summary>Add stock to a warehouse (purchase / receipt).</summary>
        [HttpPost("stock-in")]
        public async Task<IActionResult> StockIn([FromBody] StockInCommand cmd, CancellationToken ct)
            => Ok(new { success = true, message = (await _mediator.Send(cmd, ct)).Value });

        /// <summary>Remove stock from a warehouse (sale / dispatch).</summary>
        [HttpPost("stock-out")]
        public async Task<IActionResult> StockOut([FromBody] StockOutCommand cmd, CancellationToken ct)
            => Ok(new { success = true, message = (await _mediator.Send(cmd, ct)).Value });

        /// <summary>Transfer stock between two warehouses.</summary>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferCommand cmd, CancellationToken ct)
            => Ok(new { success = true, message = (await _mediator.Send(cmd, ct)).Value });

        /// <summary>Place a hold (reserve) on stock in a warehouse.</summary>
        [HttpPost("hold")]
        public async Task<IActionResult> Hold([FromBody] HoldStockCommand cmd, CancellationToken ct)
            => Ok(new { success = true, message = (await _mediator.Send(cmd, ct)).Value });

        /// <summary>Apply a stock adjustment (positive or negative delta).</summary>
        [HttpPost("adjustment")]
        public async Task<IActionResult> Adjustment([FromBody] AdjustmentCommand cmd, CancellationToken ct)
            => Ok(new { success = true, message = (await _mediator.Send(cmd, ct)).Value });
    }
}
