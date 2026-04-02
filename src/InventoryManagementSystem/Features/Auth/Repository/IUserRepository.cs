// Features/Auth/Repository/IUserRepository.cs
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Auth.Repository
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task InsertAsync(AppUser user, CancellationToken ct = default);
    }
}
