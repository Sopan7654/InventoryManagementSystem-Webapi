// Features/Reports/Queries/GetOverstockReport/GetOverstockReportQueryHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Inventory.Repository;

namespace InventoryManagementSystem.Features.Reports.Queries.GetOverstockReport
{
    public sealed class GetOverstockReportQueryHandler
        : IRequestHandler<GetOverstockReportQuery, Result<List<OverstockItem>>>
    {
        private readonly IStockLevelRepository _stockRepo;

        public GetOverstockReportQueryHandler(IStockLevelRepository stockRepo)
            => _stockRepo = stockRepo;

        public async Task<Result<List<OverstockItem>>> Handle(
            GetOverstockReportQuery query, CancellationToken ct)
        {
            var allLevels = await _stockRepo.GetAllAsync(ct);

            var overstocked = allLevels
                .Where(sl => sl.ReorderLevel > 0 &&
                             sl.QuantityOnHand > sl.ReorderLevel * query.ThresholdMultiplier)
                .Select(sl => new OverstockItem
                {
                    ProductId         = sl.ProductId,
                    ProductName       = sl.ProductName,
                    WarehouseName     = sl.WarehouseName,
                    QuantityOnHand    = sl.QuantityOnHand,
                    ReorderLevel      = sl.ReorderLevel,
                    OverstockThreshold = sl.ReorderLevel * query.ThresholdMultiplier
                })
                .OrderByDescending(x => x.ExcessQuantity)
                .ToList();

            return Result<List<OverstockItem>>.Success(overstocked);
        }
    }
}
