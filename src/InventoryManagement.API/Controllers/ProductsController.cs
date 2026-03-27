// ============================================================
// FILE: src/InventoryManagement.API/Controllers/ProductsController.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Products.Commands;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Authorize]
    public class ProductsController : BaseApiController
    {
        /// <summary>Get all products with pagination.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
            => FromResult(await Mediator.Send(new GetAllProductsQuery(pagination)));

        /// <summary>Get a product by ID.</summary>
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetById(string id)
            => FromResult(await Mediator.Send(new GetProductByIdQuery(id)));

        /// <summary>Get a product by SKU.</summary>
        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySKU(string sku)
            => FromResult(await Mediator.Send(new GetProductBySKUQuery(sku)));

        /// <summary>Create a new product.</summary>
        [HttpPost]
        [Authorize(Policy = Constants.Policies.CanManageProducts)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await Mediator.Send(new CreateProductCommand(dto));
            return CreatedFromResult(result, "GetProductById", new { id = result.Value?.ProductId });
        }

        /// <summary>Update an existing product.</summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Constants.Policies.CanManageProducts)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDto dto)
            => FromResult(await Mediator.Send(new UpdateProductCommand(id, dto)));

        /// <summary>Soft-delete a product.</summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Constants.Policies.CanManageProducts)]
        public async Task<IActionResult> Delete(string id)
            => FromResult(await Mediator.Send(new DeleteProductCommand(id)));
    }
}
