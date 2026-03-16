// Features/Reports/Queries/GetLowStockReport/GetLowStockReportQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Reports.Queries.GetLowStockReport
{ public sealed record GetLowStockReportQuery : IRequest<Result<List<StockLevel>>>; }
