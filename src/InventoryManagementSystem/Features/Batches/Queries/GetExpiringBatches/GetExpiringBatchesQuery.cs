// Features/Batches/Queries/GetExpiringBatches/GetExpiringBatchesQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Batches.Queries.GetExpiringBatches
{ public sealed record GetExpiringBatchesQuery(int Days = 30) : IRequest<Result<List<Batch>>>; }
