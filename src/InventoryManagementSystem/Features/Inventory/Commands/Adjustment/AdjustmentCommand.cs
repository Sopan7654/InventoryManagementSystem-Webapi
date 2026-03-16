// Features/Inventory/Commands/Adjustment/AdjustmentCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Inventory.Commands.Adjustment
{ public sealed record AdjustmentCommand(string ProductId, string WarehouseId, decimal Quantity, string? Reason) : IRequest<Result<string>>; }
