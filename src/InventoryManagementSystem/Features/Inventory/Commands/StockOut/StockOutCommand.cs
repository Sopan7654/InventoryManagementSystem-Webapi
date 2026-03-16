// Features/Inventory/Commands/StockOut/StockOutCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Inventory.Commands.StockOut
{ public sealed record StockOutCommand(string ProductId, string WarehouseId, decimal Quantity) : IRequest<Result<string>>; }
