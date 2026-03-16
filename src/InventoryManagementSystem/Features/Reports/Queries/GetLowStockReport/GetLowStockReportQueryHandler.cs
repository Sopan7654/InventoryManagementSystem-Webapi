// Features/Reports/Queries/GetLowStockReport/GetLowStockReportQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Inventory.Repository;
namespace InventoryManagementSystem.Features.Reports.Queries.GetLowStockReport
{
    public sealed class GetLowStockReportQueryHandler : IRequestHandler<GetLowStockReportQuery, Result<List<StockLevel>>>
    {
        private readonly IStockLevelRepository _repo;
        public GetLowStockReportQueryHandler(IStockLevelRepository repo) => _repo = repo;
        public async Task<Result<List<StockLevel>>> Handle(GetLowStockReportQuery req, CancellationToken ct)
            => Result<List<StockLevel>>.Success(await _repo.GetLowStockAsync(ct));
    }
}
