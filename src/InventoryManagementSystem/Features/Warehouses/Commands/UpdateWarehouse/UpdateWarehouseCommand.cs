// Features/Warehouses/Commands/UpdateWarehouse/UpdateWarehouseCommand.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Warehouses.Commands.UpdateWarehouse
{
    public record UpdateWarehouseCommand(
        string WarehouseName,
        string? Location,
        decimal? Capacity,
        string WarehouseId = ""
    ) : IRequest<Result<string>>;
}
