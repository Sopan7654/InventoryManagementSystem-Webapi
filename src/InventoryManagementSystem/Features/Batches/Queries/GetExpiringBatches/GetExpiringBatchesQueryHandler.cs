// Features/Batches/Queries/GetExpiringBatches/GetExpiringBatchesQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Batches.Repository;
namespace InventoryManagementSystem.Features.Batches.Queries.GetExpiringBatches
{
    public sealed class GetExpiringBatchesQueryHandler : IRequestHandler<GetExpiringBatchesQuery, Result<List<Batch>>>
    {
        private readonly IBatchRepository _repo;
        public GetExpiringBatchesQueryHandler(IBatchRepository repo) => _repo = repo;
        public async Task<Result<List<Batch>>> Handle(GetExpiringBatchesQuery req, CancellationToken ct)
            => Result<List<Batch>>.Success(await _repo.GetExpiringSoonAsync(req.Days, ct));
    }
}
