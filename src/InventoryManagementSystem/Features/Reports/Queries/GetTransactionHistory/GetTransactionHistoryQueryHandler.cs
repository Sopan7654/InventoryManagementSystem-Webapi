// Features/Reports/Queries/GetTransactionHistory/GetTransactionHistoryQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Inventory.Repository;
namespace InventoryManagementSystem.Features.Reports.Queries.GetTransactionHistory
{
    public sealed class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<List<StockTransaction>>>
    {
        private readonly IStockTransactionRepository _repo;
        public GetTransactionHistoryQueryHandler(IStockTransactionRepository repo) => _repo = repo;
        public async Task<Result<List<StockTransaction>>> Handle(GetTransactionHistoryQuery req, CancellationToken ct)
        {
            var data = req.ProductId is not null
                ? await _repo.GetByProductAsync(req.ProductId, ct)
                : await _repo.GetAllAsync(req.Limit, ct);
            return Result<List<StockTransaction>>.Success(data);
        }
    }
}
