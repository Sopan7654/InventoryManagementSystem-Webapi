// Features/Warehouses/Commands/CreateWarehouse/CreateWarehouseCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Warehouses.Repository;
namespace InventoryManagementSystem.Features.Warehouses.Commands.CreateWarehouse
{
    public sealed class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Result<string>>
    {
        private readonly IWarehouseRepository _repo;
        public CreateWarehouseCommandHandler(IWarehouseRepository repo) => _repo = repo;
        public async Task<Result<string>> Handle(CreateWarehouseCommand cmd, CancellationToken ct)
        {
            var w = new Warehouse { WarehouseId = Guid.NewGuid().ToString(), WarehouseName = cmd.WarehouseName, Location = cmd.Location, Capacity = cmd.Capacity };
            await _repo.InsertAsync(w, ct);
            return Result<string>.Success(w.WarehouseId);
        }
    }
}
