// Features/Categories/CategoriesController.cs
using MediatR; using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Features.Categories.Commands.CreateCategory;
using InventoryManagementSystem.Features.Categories.Queries.GetAllCategories;
namespace InventoryManagementSystem.Features.Categories
{
    [ApiController][Route("api/[controller]")][Produces("application/json")]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(new { success = true, data = (await _mediator.Send(new GetAllCategoriesQuery(), ct)).Value });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return StatusCode(201, new { success = true, categoryId = result.Value });
        }
    }
}
