// Features/Batches/Queries/GetAllBatches/GetAllBatchesQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Batches.Repository;
namespace InventoryManagementSystem.Features.Batches.Queries.GetAllBatches
{
    public sealed class GetAllBatchesQueryHandler : IRequestHandler<GetAllBatchesQuery, Result<List<Batch>>>
    {
        private readonly IBatchRepository _repo;
        public GetAllBatchesQueryHandler(IBatchRepository repo) => _repo = repo;
        public async Task<Result<List<Batch>>> Handle(GetAllBatchesQuery req, CancellationToken ct)
            => Result<List<Batch>>.Success(await _repo.GetAllAsync(ct));
    }
}
