// Features/Reports/Queries/GetTransactionHistory/GetTransactionHistoryQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Reports.Queries.GetTransactionHistory
{ public sealed record GetTransactionHistoryQuery(string? ProductId = null, int Limit = 50) : IRequest<Result<List<StockTransaction>>>; }
