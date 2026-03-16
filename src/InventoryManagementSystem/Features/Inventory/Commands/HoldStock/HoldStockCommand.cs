// Features/Inventory/Commands/HoldStock/HoldStockCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Inventory.Commands.HoldStock
{ public sealed record HoldStockCommand(string ProductId, string WarehouseId, decimal Quantity) : IRequest<Result<string>>; }
