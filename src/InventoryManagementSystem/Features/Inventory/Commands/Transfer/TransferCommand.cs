// Features/Inventory/Commands/Transfer/TransferCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Inventory.Commands.Transfer
{ public sealed record TransferCommand(string ProductId, string FromWarehouseId, string ToWarehouseId, decimal Quantity) : IRequest<Result<string>>; }
