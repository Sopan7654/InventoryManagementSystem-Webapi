// Features/Categories/Repository/ICategoryRepository.cs
using InventoryManagementSystem.Common.Interfaces; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Categories.Repository
{
    public interface ICategoryRepository : IRepository<ProductCategory>
    {
        Task<List<ProductCategory>> GetAllAsync(CancellationToken ct = default);
        Task<ProductCategory?> GetByIdAsync(string id, CancellationToken ct = default);
        Task InsertAsync(ProductCategory category, CancellationToken ct = default);
    }
}
