// Features/Warehouses/Commands/CreateWarehouse/CreateWarehouseCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Warehouses.Commands.CreateWarehouse
{ public sealed record CreateWarehouseCommand(string WarehouseName, string? Location, decimal? Capacity) : IRequest<Result<string>>; }
