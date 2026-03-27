// ============================================================
// FILE: src/InventoryManagement.API/Controllers/PurchaseOrdersController.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.PurchaseOrders;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Authorize]
    public class PurchaseOrdersController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
            => FromResult(await Mediator.Send(new GetAllPurchaseOrdersQuery(pagination)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
            => FromResult(await Mediator.Send(new GetPurchaseOrderByIdQuery(id)));

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
            => FromResult(await Mediator.Send(new CreatePurchaseOrderCommand(dto)));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePurchaseOrderDto dto)
            => FromResult(await Mediator.Send(new UpdatePurchaseOrderCommand(id, dto)));

        /// <summary>Approve a pending purchase order.</summary>
        [HttpPatch("{id}/approve")]
        [Authorize(Policy = Constants.Policies.CanApproveOrders)]
        public async Task<IActionResult> Approve(string id)
            => FromResult(await Mediator.Send(new ApprovePurchaseOrderCommand(id)));

        /// <summary>Receive an approved PO into a warehouse.</summary>
        [HttpPatch("{id}/receive")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<IActionResult> Receive(string id, [FromQuery] string warehouseId)
            => FromResult(await Mediator.Send(new ReceivePurchaseOrderCommand(id, warehouseId)));
    }
}
