// Features/Products/Commands/DeleteProduct/DeleteProductCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Commands.DeleteProduct
{
    /// <summary>
    /// Soft-deletes a product — sets IsActive = false.
    /// SOLID: SRP — this handler does exactly one thing.
    /// </summary>
    public sealed class DeleteProductCommandHandler
        : IRequestHandler<DeleteProductCommand, Result<string>>
    {
        private readonly IProductRepository _productRepo;
        private readonly ICacheService _cache;

        public DeleteProductCommandHandler(IProductRepository productRepo, ICacheService cache)
        {
            _productRepo = productRepo;
            _cache = cache;
        }

        public async Task<Result<string>> Handle(DeleteProductCommand cmd, CancellationToken ct)
        {
            var product = await _productRepo.GetByIdAsync(cmd.ProductId, ct)
                ?? throw new NotFoundException($"Product '{cmd.ProductId}' not found.");

            if (!product.IsActive)
                return Result<string>.Success("Product is already inactive.");

            product.IsActive = false;
            var updated = await _productRepo.UpdateAsync(product, ct);
            await _cache.RemoveAsync("products_all", ct);

            return updated
                ? Result<string>.Success($"Product '{product.ProductName}' has been deactivated.")
                : Result<string>.Failure("Failed to deactivate product.");
        }
    }
}
