// Features/Batches/Queries/GetAllBatches/GetAllBatchesQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Batches.Queries.GetAllBatches
{ public sealed record GetAllBatchesQuery : IRequest<Result<List<Batch>>>; }
