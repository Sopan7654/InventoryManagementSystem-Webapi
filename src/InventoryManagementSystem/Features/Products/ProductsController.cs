// Features/Products/ProductsController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Products.Commands.CreateProduct;
using InventoryManagementSystem.Features.Products.Commands.UpdateProduct;
using InventoryManagementSystem.Features.Products.Queries.GetAllProducts;
using InventoryManagementSystem.Features.Products.Queries.GetProductById;

namespace InventoryManagementSystem.Features.Products
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator) => _mediator = mediator;

        /// <summary>Get all products.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllProductsQuery(), ct);
            return Ok(new { success = true, data = result.Value });
        }

        /// <summary>Get a product by ID.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
            return Ok(new { success = true, data = result.Value });
        }

        /// <summary>Create a new product.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Value },
                new { success = true, productId = result.Value });
        }

        /// <summary>Update an existing product.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductCommand cmd, CancellationToken ct)
        {
            var updatedCmd = cmd with { ProductId = id };
            var result = await _mediator.Send(updatedCmd, ct);
            return Ok(new { success = true, updated = result.Value });
        }
    }
}
