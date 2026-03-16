// Features/PurchaseOrders/Commands/CreatePurchaseOrder/CreatePurchaseOrderCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.CreatePurchaseOrder
{
    public sealed record PurchaseOrderItemDto(string ProductId, decimal QuantityOrdered, decimal UnitPrice);
    public sealed record CreatePurchaseOrderCommand(string SupplierId, DateTime? OrderDate, List<PurchaseOrderItemDto> Items) : IRequest<Result<string>>;
}
