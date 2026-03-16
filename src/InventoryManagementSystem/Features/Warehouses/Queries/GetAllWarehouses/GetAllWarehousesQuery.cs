// Features/Warehouses/Queries/GetAllWarehouses/GetAllWarehousesQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Warehouses.Queries.GetAllWarehouses
{ public sealed record GetAllWarehousesQuery : IRequest<Result<List<Warehouse>>>; }
