// Features/Warehouses/Commands/UpdateWarehouse/UpdateWarehouseCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Warehouses.Repository;

namespace InventoryManagementSystem.Features.Warehouses.Commands.UpdateWarehouse
{
    public sealed class UpdateWarehouseCommandHandler
        : IRequestHandler<UpdateWarehouseCommand, Result<string>>
    {
        private readonly IWarehouseRepository _warehouseRepo;

        public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepo)
            => _warehouseRepo = warehouseRepo;

        public async Task<Result<string>> Handle(UpdateWarehouseCommand cmd, CancellationToken ct)
        {
            var warehouse = await _warehouseRepo.GetByIdAsync(cmd.WarehouseId, ct)
                ?? throw new NotFoundException($"Warehouse '{cmd.WarehouseId}' not found.");

            warehouse.WarehouseName = cmd.WarehouseName;
            warehouse.Location      = cmd.Location;
            warehouse.Capacity      = cmd.Capacity;

            var updated = await _warehouseRepo.UpdateAsync(warehouse, ct);

            return updated
                ? Result<string>.Success($"Warehouse '{warehouse.WarehouseName}' updated successfully.")
                : Result<string>.Failure("Failed to update warehouse.");
        }
    }
}
