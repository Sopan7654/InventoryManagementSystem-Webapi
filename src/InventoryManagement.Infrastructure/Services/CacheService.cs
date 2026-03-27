// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/CacheService.cs
// ============================================================
using System.Text.Json;
using InventoryManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace InventoryManagement.Infrastructure.Services
{
    /// <summary>Redis-backed cache service (Singleton).</summary>
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger)
        {
            _redis = redis;
            _db = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            try
            {
                RedisValue value = await _db.StringGetAsync(key);
                if (value.IsNullOrEmpty) return default;
                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache GET failed for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
        {
            try
            {
                string json = JsonSerializer.Serialize(value);
                await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache SET failed for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            try { await _db.KeyDeleteAsync(key); }
            catch (Exception ex) { _logger.LogWarning(ex, "Cache REMOVE failed for key: {Key}", key); }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
        {
            try
            {
                var endpoints = _redis.GetEndPoints();
                var server = _redis.GetServer(endpoints.First());
                var keys = server.Keys(pattern: pattern).ToArray();
                if (keys.Length > 0) await _db.KeyDeleteAsync(keys);
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Cache REMOVE-BY-PATTERN failed: {Pattern}", pattern); }
        }
    }
}
