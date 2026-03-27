// ============================================================
// FILE: src/InventoryManagement.Application/Features/Products/Queries/ProductQueries.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Products.Queries
{
    /// <summary>Query to get all products with pagination.</summary>
    public record GetAllProductsQuery(PaginationParams Pagination) : IRequest<Result<PaginatedResponse<ProductSummaryDto>>>, ICacheable
    {
        public string CacheKey => $"{Constants.CacheKeys.AllProducts}:{Pagination.PageNumber}:{Pagination.PageSize}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(10);
    }

    /// <summary>Query to get a product by ID.</summary>
    public record GetProductByIdQuery(string ProductId) : IRequest<Result<ProductResponseDto>>, ICacheable
    {
        public string CacheKey => string.Format(Constants.CacheKeys.ProductById, ProductId);
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(15);
    }

    /// <summary>Query to get a product by SKU.</summary>
    public record GetProductBySKUQuery(string SKU) : IRequest<Result<ProductResponseDto>>, ICacheable
    {
        public string CacheKey => string.Format(Constants.CacheKeys.ProductBySku, SKU);
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(15);
    }
}
