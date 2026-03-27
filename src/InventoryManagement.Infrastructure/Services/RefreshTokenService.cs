// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/RefreshTokenService.cs
// ============================================================
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;

        public RefreshTokenService(IUnitOfWork uow, IJwtService jwtService)
        { _uow = uow; _jwtService = jwtService; }

        public async Task<string> GenerateAndStoreRefreshTokenAsync(User user, CancellationToken ct = default)
        {
            string refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);
            return refreshToken;
        }

        public async Task<User?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            User? user = await _uow.Users.GetByRefreshTokenAsync(refreshToken, ct);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return null;
            return user;
        }

        public async Task RevokeRefreshTokenAsync(User user, CancellationToken ct = default)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            _uow.Users.Update(user);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
