// Features/PurchaseOrders/Queries/GetAllPurchaseOrders/GetAllPurchaseOrdersQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.PurchaseOrders.Queries.GetAllPurchaseOrders
{ public sealed record GetAllPurchaseOrdersQuery : IRequest<Result<List<PurchaseOrder>>>; }
