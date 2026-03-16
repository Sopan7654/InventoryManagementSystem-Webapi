// Features/Warehouses/WarehousesController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Warehouses.Commands.CreateWarehouse;
using InventoryManagementSystem.Features.Warehouses.Queries.GetAllWarehouses;
using InventoryManagementSystem.Features.Warehouses.Queries.GetWarehouseById;
namespace InventoryManagementSystem.Features.Warehouses
{
    [ApiController][Route("api/[controller]")][Produces("application/json")]
    public sealed class WarehousesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WarehousesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetAllWarehousesQuery(), ct)).Value });

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetWarehouseByIdQuery(id), ct)).Value });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { success = true, warehouseId = result.Value });
        }
    }
}
