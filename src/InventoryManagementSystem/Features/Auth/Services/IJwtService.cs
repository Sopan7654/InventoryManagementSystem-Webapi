// Features/Auth/Services/IJwtService.cs
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Auth.Services
{
    public interface IJwtService
    {
        string GenerateToken(AppUser user);
    }
}
