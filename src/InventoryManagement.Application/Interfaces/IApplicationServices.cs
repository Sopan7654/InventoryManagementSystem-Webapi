// ============================================================
// FILE: src/InventoryManagement.Application/Interfaces/IApplicationServices.cs
// ============================================================
using InventoryManagement.Domain.Models;

namespace InventoryManagement.Application.Interfaces
{
    /// <summary>Provides caching operations (Singleton).</summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    }

    /// <summary>Generates and validates JWT tokens.</summary>
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }

    /// <summary>Manages refresh tokens.</summary>
    public interface IRefreshTokenService
    {
        Task<string> GenerateAndStoreRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(User user, CancellationToken cancellationToken = default);
    }

    /// <summary>Manages blacklisted JWT access tokens (Singleton).</summary>
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string jti, TimeSpan expiry, CancellationToken cancellationToken = default);
        Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken cancellationToken = default);
    }

    /// <summary>Sends email notifications (Transient).</summary>
    public interface IEmailNotificationService
    {
        Task SendLowStockAlertAsync(string productId, string warehouseId, decimal currentQuantity, decimal reorderLevel, CancellationToken cancellationToken = default);
        Task SendBatchExpiryAlertAsync(string batchId, string productName, DateTime expiryDate, CancellationToken cancellationToken = default);
    }

    /// <summary>Validation service for cross-cutting validation concerns (Transient).</summary>
    public interface IValidationService
    {
        Task<bool> ProductExistsAsync(string productId, CancellationToken cancellationToken = default);
        Task<bool> WarehouseExistsAsync(string warehouseId, CancellationToken cancellationToken = default);
        Task<bool> SupplierExistsAsync(string supplierId, CancellationToken cancellationToken = default);
    }

    /// <summary>API Gateway aggregation service.</summary>
    public interface IApiGatewayService
    {
        Task<DTOs.DashboardResponseDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>Marker interface for cacheable MediatR queries.</summary>
    public interface ICacheable
    {
        string CacheKey { get; }
        TimeSpan? CacheDuration { get; }
    }
}
