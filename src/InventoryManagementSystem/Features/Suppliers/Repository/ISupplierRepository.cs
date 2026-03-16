// Features/Suppliers/Repository/ISupplierRepository.cs
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Suppliers.Repository
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<List<Supplier>> GetAllAsync(CancellationToken ct = default);
        Task<Supplier?> GetByIdAsync(string id, CancellationToken ct = default);
        Task InsertAsync(Supplier supplier, CancellationToken ct = default);
        Task<bool> UpdateAsync(Supplier supplier, CancellationToken ct = default);
    }
}
