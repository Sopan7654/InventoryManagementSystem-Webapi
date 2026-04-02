// Features/Warehouses/Repository/IWarehouseRepository.cs
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Warehouses.Repository
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<List<Warehouse>> GetAllAsync(CancellationToken ct = default);
        Task<Warehouse?> GetByIdAsync(string id, CancellationToken ct = default);
        Task InsertAsync(Warehouse warehouse, CancellationToken ct = default);
        Task<bool> UpdateAsync(Warehouse warehouse, CancellationToken ct = default);
    }
}
