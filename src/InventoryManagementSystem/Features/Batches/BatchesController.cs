// Features/Batches/BatchesController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Batches.Commands.CreateBatch;
using InventoryManagementSystem.Features.Batches.Queries.GetAllBatches;
using InventoryManagementSystem.Features.Batches.Queries.GetExpiringBatches;
namespace InventoryManagementSystem.Features.Batches
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class BatchesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BatchesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetAllBatchesQuery(), ct)).Value });

        [HttpGet("expiring")]
        public async Task<IActionResult> GetExpiring([FromQuery] int days = 30, CancellationToken ct = default)
            => Ok(new { success = true, data = (await _mediator.Send(new GetExpiringBatchesQuery(days), ct)).Value });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBatchCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return StatusCode(201, new { success = true, batchId = result.Value });
        }
    }
}
