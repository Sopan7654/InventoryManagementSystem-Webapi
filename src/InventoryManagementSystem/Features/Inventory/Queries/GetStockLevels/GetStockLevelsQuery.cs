// Features/Inventory/Queries/GetStockLevels/GetStockLevelsQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Inventory.Queries.GetStockLevels
{ public sealed record GetStockLevelsQuery : IRequest<Result<List<StockLevel>>>; }
