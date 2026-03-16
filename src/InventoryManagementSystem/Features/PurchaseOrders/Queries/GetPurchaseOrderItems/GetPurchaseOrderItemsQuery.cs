// Features/PurchaseOrders/Queries/GetPurchaseOrderItems/GetPurchaseOrderItemsQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.PurchaseOrders.Queries.GetPurchaseOrderItems
{ public sealed record GetPurchaseOrderItemsQuery(string PurchaseOrderId) : IRequest<Result<List<PurchaseOrderItem>>>; }
