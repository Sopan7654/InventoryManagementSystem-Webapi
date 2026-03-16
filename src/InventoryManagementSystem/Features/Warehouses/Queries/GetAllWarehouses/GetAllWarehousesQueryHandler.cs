// Features/Warehouses/Queries/GetAllWarehouses/GetAllWarehousesQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Warehouses.Repository;
namespace InventoryManagementSystem.Features.Warehouses.Queries.GetAllWarehouses
{
    public sealed class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, Result<List<Warehouse>>>
    {
        private readonly IWarehouseRepository _repo;
        public GetAllWarehousesQueryHandler(IWarehouseRepository repo) => _repo = repo;
        public async Task<Result<List<Warehouse>>> Handle(GetAllWarehousesQuery req, CancellationToken ct)
            => Result<List<Warehouse>>.Success(await _repo.GetAllAsync(ct));
    }
}
