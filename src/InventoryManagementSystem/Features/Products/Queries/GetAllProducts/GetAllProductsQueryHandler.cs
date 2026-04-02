using MediatR;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Queries.GetAllProducts
{
    public sealed class GetAllProductsQueryHandler
        : IRequestHandler<GetAllProductsQuery, Result<List<Product>>>
    {
        private readonly IProductRepository _repo;
        private readonly ICacheService _cache;
        private const string CacheKey = "products_all";

        public GetAllProductsQueryHandler(IProductRepository repo, ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<Result<List<Product>>> Handle(
            GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var cachedProducts = await _cache.GetAsync<List<Product>>(CacheKey, cancellationToken);
            if (cachedProducts != null)
            {
                return Result<List<Product>>.Success(cachedProducts);
            }

            var products = await _repo.GetAllAsync(cancellationToken);
            
            await _cache.SetAsync(
                CacheKey, 
                products, 
                absoluteExpireTime: TimeSpan.FromMinutes(5), 
                ct: cancellationToken);

            return Result<List<Product>>.Success(products);
        }
    }
}
