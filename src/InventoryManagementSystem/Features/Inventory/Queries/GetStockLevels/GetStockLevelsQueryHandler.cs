// Features/Inventory/Queries/GetStockLevels/GetStockLevelsQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Inventory.Repository;
namespace InventoryManagementSystem.Features.Inventory.Queries.GetStockLevels
{
    public sealed class GetStockLevelsQueryHandler : IRequestHandler<GetStockLevelsQuery, Result<List<StockLevel>>>
    {
        private readonly IStockLevelRepository _repo;
        public GetStockLevelsQueryHandler(IStockLevelRepository repo) => _repo = repo;
        public async Task<Result<List<StockLevel>>> Handle(GetStockLevelsQuery req, CancellationToken ct)
            => Result<List<StockLevel>>.Success(await _repo.GetAllAsync(ct));
    }
}
