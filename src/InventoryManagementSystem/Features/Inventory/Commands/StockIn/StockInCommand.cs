// Features/Inventory/Commands/StockIn/StockInCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Inventory.Commands.StockIn
{
    /// <summary>CQRS Command — add stock to a warehouse. Uses Unit of Work for atomicity.</summary>
    public sealed record StockInCommand(string ProductId, string WarehouseId, decimal Quantity) : IRequest<Result<string>>;
}
