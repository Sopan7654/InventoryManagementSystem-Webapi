// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/TokenBlacklistService.cs
// ============================================================
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Shared;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace InventoryManagement.Infrastructure.Services
{
    /// <summary>Redis-backed JWT token blacklist (Singleton).</summary>
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDatabase _db;
        private readonly ILogger<TokenBlacklistService> _logger;

        public TokenBlacklistService(IConnectionMultiplexer redis, ILogger<TokenBlacklistService> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task BlacklistTokenAsync(string jti, TimeSpan expiry, CancellationToken ct = default)
        {
            try
            {
                string key = string.Format(Constants.CacheKeys.TokenBlacklist, jti);
                await _db.StringSetAsync(key, "blacklisted", expiry);
                _logger.LogInformation("Token blacklisted: {Jti}", jti);
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to blacklist token: {Jti}", jti); }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken ct = default)
        {
            try
            {
                string key = string.Format(Constants.CacheKeys.TokenBlacklist, jti);
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check token blacklist: {Jti}", jti);
                return false;
            }
        }
    }
}
