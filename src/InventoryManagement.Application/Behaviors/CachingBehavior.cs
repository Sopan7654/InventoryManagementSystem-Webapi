// ============================================================
// FILE: src/InventoryManagement.Application/Behaviors/CachingBehavior.cs
// ============================================================
using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that caches responses for queries implementing ICacheable.
    /// </summary>
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(ICacheService cacheService,
            ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICacheable cacheable)
                return await next();

            TResponse? cached = await _cacheService.GetAsync<TResponse>(cacheable.CacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogDebug("Cache HIT for key: {CacheKey}", cacheable.CacheKey);
                return cached;
            }

            _logger.LogDebug("Cache MISS for key: {CacheKey}", cacheable.CacheKey);
            TResponse response = await next();

            await _cacheService.SetAsync(cacheable.CacheKey, response, cacheable.CacheDuration, cancellationToken);
            return response;
        }
    }
}
