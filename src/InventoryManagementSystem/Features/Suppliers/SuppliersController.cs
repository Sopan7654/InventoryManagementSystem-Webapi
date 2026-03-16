// Features/Suppliers/SuppliersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Suppliers.Commands.CreateSupplier;
using InventoryManagementSystem.Features.Suppliers.Commands.UpdateSupplier;
using InventoryManagementSystem.Features.Suppliers.Queries.GetAllSuppliers;
using InventoryManagementSystem.Features.Suppliers.Queries.GetSupplierById;

namespace InventoryManagementSystem.Features.Suppliers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class SuppliersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SuppliersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetAllSuppliersQuery(), ct)).Value });

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetSupplierByIdQuery(id), ct)).Value });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { success = true, supplierId = result.Value });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSupplierCommand cmd, CancellationToken ct)
        {
            var updatedCmd = cmd with { SupplierId = id };
            await _mediator.Send(updatedCmd, ct);
            return Ok(new { success = true });
        }
    }
}
