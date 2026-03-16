// Features/Products/Repository/IProductRepository.cs
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Products.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllAsync(CancellationToken ct = default);
        Task<Product?> GetByIdAsync(string productId, CancellationToken ct = default);
        Task<Product?> GetBySKUAsync(string sku, CancellationToken ct = default);
        Task InsertAsync(Product product, CancellationToken ct = default);
        Task<bool> UpdateAsync(Product product, CancellationToken ct = default);
        Task<bool> SKUExistsAsync(string sku, string? excludeId = null, CancellationToken ct = default);
    }
}
