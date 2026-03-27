// ============================================================
// FILE: src/InventoryManagement.API/Controllers/DashboardController.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Batches;
using InventoryManagement.Application.Features.Inventory.Queries;
using InventoryManagement.Application.Features.Products.Queries;
using InventoryManagement.Application.Features.Warehouses;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace InventoryManagement.API.Controllers
{
    /// <summary>API Gateway pattern — aggregates data from multiple services.</summary>
    [Authorize]
    public class DashboardController : BaseApiController
    {
        [HttpGet]
        [Authorize(Policy = Constants.Policies.CanViewReports)]
        public async Task<IActionResult> GetDashboard()
        {
            // Aggregate calls (API Gateway pattern)
            var productsTask = Mediator.Send(new GetAllProductsQuery(new PaginationParams { PageSize = 1 }));
            var warehousesTask = Mediator.Send(new GetAllWarehousesQuery(new PaginationParams { PageSize = 1 }));
            var lowStockTask = Mediator.Send(new GetLowStockReportQuery());
            var expiringTask = Mediator.Send(new GetExpiringBatchesQuery(30));

            await Task.WhenAll(productsTask, warehousesTask, lowStockTask, expiringTask);

            var dashboard = new DashboardResponseDto
            {
                TotalProducts = productsTask.Result.IsSuccess ? productsTask.Result.Value!.TotalCount : 0,
                TotalWarehouses = warehousesTask.Result.IsSuccess ? warehousesTask.Result.Value!.TotalCount : 0,
                LowStockCount = lowStockTask.Result.IsSuccess ? lowStockTask.Result.Value!.Count() : 0,
                ExpiringBatchesCount = expiringTask.Result.IsSuccess ? expiringTask.Result.Value!.Count() : 0
            };

            return Ok(dashboard);
        }
    }
}
