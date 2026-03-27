// ============================================================
// Controllers: Categories, Suppliers, Warehouses, Batches
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Categories;
using InventoryManagement.Application.Features.Suppliers;
using InventoryManagement.Application.Features.Warehouses;
using InventoryManagement.Application.Features.Batches;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    [Authorize]
    public class CategoriesController : BaseApiController
    {
        [HttpGet] public async Task<IActionResult> GetAll() => FromResult(await Mediator.Send(new GetAllCategoriesQuery()));
        [HttpGet("{id}")] public async Task<IActionResult> GetById(string id) => FromResult(await Mediator.Send(new GetCategoryByIdQuery(id)));
        [HttpPost] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
            => FromResult(await Mediator.Send(new CreateCategoryCommand(dto)));
        [HttpPut("{id}")] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Update(string id, [FromBody] UpdateCategoryDto dto)
            => FromResult(await Mediator.Send(new UpdateCategoryCommand(id, dto)));
        [HttpDelete("{id}")] [Authorize(Roles = "Admin")] public async Task<IActionResult> Delete(string id)
            => FromResult(await Mediator.Send(new DeleteCategoryCommand(id)));
    }

    [Authorize]
    public class SuppliersController : BaseApiController
    {
        [HttpGet] public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
            => FromResult(await Mediator.Send(new GetAllSuppliersQuery(pagination)));
        [HttpGet("{id}")] public async Task<IActionResult> GetById(string id)
            => FromResult(await Mediator.Send(new GetSupplierByIdQuery(id)));
        [HttpPost] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
            => FromResult(await Mediator.Send(new CreateSupplierCommand(dto)));
        [HttpPut("{id}")] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Update(string id, [FromBody] UpdateSupplierDto dto)
            => FromResult(await Mediator.Send(new UpdateSupplierCommand(id, dto)));
        [HttpDelete("{id}")] [Authorize(Roles = "Admin")] public async Task<IActionResult> Delete(string id)
            => FromResult(await Mediator.Send(new DeleteSupplierCommand(id)));
    }

    [Authorize]
    public class WarehousesController : BaseApiController
    {
        [HttpGet] public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination)
            => FromResult(await Mediator.Send(new GetAllWarehousesQuery(pagination)));
        [HttpGet("{id}")] public async Task<IActionResult> GetById(string id)
            => FromResult(await Mediator.Send(new GetWarehouseByIdQuery(id)));
        [HttpPost] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
            => FromResult(await Mediator.Send(new CreateWarehouseCommand(dto)));
        [HttpPut("{id}")] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Update(string id, [FromBody] UpdateWarehouseDto dto)
            => FromResult(await Mediator.Send(new UpdateWarehouseCommand(id, dto)));
    }

    [Authorize]
    public class BatchesController : BaseApiController
    {
        [HttpGet("product/{productId}")] public async Task<IActionResult> GetByProduct(string productId)
            => FromResult(await Mediator.Send(new GetBatchesByProductQuery(productId)));
        [HttpGet("expiring")] public async Task<IActionResult> GetExpiring([FromQuery] int daysAhead = 30)
            => FromResult(await Mediator.Send(new GetExpiringBatchesQuery(daysAhead)));
        [HttpPost] [Authorize(Roles = "Admin,Manager,Operator")] public async Task<IActionResult> Create([FromBody] CreateBatchDto dto)
            => FromResult(await Mediator.Send(new CreateBatchCommand(dto)));
        [HttpPut("{id}")] [Authorize(Roles = "Admin,Manager")] public async Task<IActionResult> Update(string id, [FromBody] UpdateBatchDto dto)
            => FromResult(await Mediator.Send(new UpdateBatchCommand(id, dto)));
    }
}
