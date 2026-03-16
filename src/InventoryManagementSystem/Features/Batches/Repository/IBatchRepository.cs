// Features/Batches/Repository/IBatchRepository.cs
using InventoryManagementSystem.Common.Interfaces; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Batches.Repository
{
    public interface IBatchRepository : IRepository<Batch>
    {
        Task<List<Batch>> GetAllAsync(CancellationToken ct = default);
        Task<List<Batch>> GetExpiringSoonAsync(int days = 30, CancellationToken ct = default);
        Task InsertAsync(Batch batch, CancellationToken ct = default);
    }
}
