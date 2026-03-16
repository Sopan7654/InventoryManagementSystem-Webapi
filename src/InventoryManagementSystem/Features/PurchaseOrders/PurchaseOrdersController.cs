// Features/PurchaseOrders/PurchaseOrdersController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.PurchaseOrders.Commands.CreatePurchaseOrder;
using InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus;
using InventoryManagementSystem.Features.PurchaseOrders.Queries.GetAllPurchaseOrders;
using InventoryManagementSystem.Features.PurchaseOrders.Queries.GetPurchaseOrderItems;
namespace InventoryManagementSystem.Features.PurchaseOrders
{
    [ApiController][Route("api/purchase-orders")][Produces("application/json")]
    public sealed class PurchaseOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PurchaseOrdersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetAllPurchaseOrdersQuery(), ct)).Value });

        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetItems(string id, CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetPurchaseOrderItemsQuery(id), ct)).Value });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return StatusCode(201, new { success = true, purchaseOrderId = result.Value });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdatePurchaseOrderStatusCommand cmd, CancellationToken ct)
        {
            await _mediator.Send(cmd with { PurchaseOrderId = id }, ct);
            return Ok(new { success = true });
        }
    }
}
