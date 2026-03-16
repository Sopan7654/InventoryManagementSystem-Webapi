// Features/Warehouses/Queries/GetWarehouseById/GetWarehouseByIdQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Exceptions; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Warehouses.Repository;
namespace InventoryManagementSystem.Features.Warehouses.Queries.GetWarehouseById
{
    public sealed class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, Result<Warehouse>>
    {
        private readonly IWarehouseRepository _repo;
        public GetWarehouseByIdQueryHandler(IWarehouseRepository repo) => _repo = repo;
        public async Task<Result<Warehouse>> Handle(GetWarehouseByIdQuery req, CancellationToken ct)
        {
            var w = await _repo.GetByIdAsync(req.WarehouseId, ct)
                ?? throw new NotFoundException(nameof(Warehouse), req.WarehouseId);
            return Result<Warehouse>.Success(w);
        }
    }
}
