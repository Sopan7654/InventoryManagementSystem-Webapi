// Features/Warehouses/Queries/GetWarehouseById/GetWarehouseByIdQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Warehouses.Queries.GetWarehouseById
{ public sealed record GetWarehouseByIdQuery(string WarehouseId) : IRequest<Result<Warehouse>>; }
