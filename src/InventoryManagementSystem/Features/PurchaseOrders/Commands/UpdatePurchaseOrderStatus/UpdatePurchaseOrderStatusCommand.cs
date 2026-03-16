// Features/PurchaseOrders/Commands/UpdatePurchaseOrderStatus/UpdatePurchaseOrderStatusCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.PurchaseOrders.Commands.UpdatePurchaseOrderStatus
{ public sealed record UpdatePurchaseOrderStatusCommand(string PurchaseOrderId, string Status) : IRequest<Result<bool>>; }
