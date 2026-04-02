// Common/Services/RedisCacheService.cs
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Common.Services
{
    public sealed class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache) => _cache = cache;

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var cachedData = await _cache.GetStringAsync(key, ct);
            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null, CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;

            if (unusedExpireTime.HasValue)
                options.SlidingExpiration = unusedExpireTime;

            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options, ct);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await _cache.RemoveAsync(key, ct);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            return await _cache.GetStringAsync(key, ct) != null;
        }
    }
}
