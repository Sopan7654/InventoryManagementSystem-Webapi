// Features/Batches/Commands/CreateBatch/CreateBatchCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Batches.Repository;
namespace InventoryManagementSystem.Features.Batches.Commands.CreateBatch
{
    public sealed class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, Result<string>>
    {
        private readonly IBatchRepository _repo;
        public CreateBatchCommandHandler(IBatchRepository repo) => _repo = repo;
        public async Task<Result<string>> Handle(CreateBatchCommand cmd, CancellationToken ct)
        {
            var batch = new Batch { BatchId = Guid.NewGuid().ToString(), ProductId = cmd.ProductId, WarehouseId = cmd.WarehouseId, BatchNumber = cmd.BatchNumber, ManufacturingDate = cmd.ManufacturingDate, ExpiryDate = cmd.ExpiryDate, Quantity = cmd.Quantity };
            await _repo.InsertAsync(batch, ct);
            return Result<string>.Success(batch.BatchId);
        }
    }
}
