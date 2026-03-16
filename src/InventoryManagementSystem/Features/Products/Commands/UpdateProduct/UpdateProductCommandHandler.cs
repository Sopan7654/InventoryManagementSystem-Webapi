// Features/Products/Commands/UpdateProduct/UpdateProductCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandHandler
        : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private readonly IProductRepository _repo;

        public UpdateProductCommandHandler(IProductRepository repo) => _repo = repo;

        public async Task<Result<bool>> Handle(
            UpdateProductCommand cmd, CancellationToken cancellationToken)
        {
            var existing = await _repo.GetByIdAsync(cmd.ProductId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Product), cmd.ProductId);

            if (await _repo.SKUExistsAsync(cmd.SKU, excludeId: cmd.ProductId, ct: cancellationToken))
                throw new DomainException($"SKU '{cmd.SKU}' is already used by another product.");

            existing.SKU           = cmd.SKU;
            existing.ProductName   = cmd.ProductName;
            existing.Description   = cmd.Description;
            existing.CategoryId    = cmd.CategoryId;
            existing.UnitOfMeasure = cmd.UnitOfMeasure;
            existing.Cost          = cmd.Cost;
            existing.ListPrice     = cmd.ListPrice;
            existing.IsActive      = cmd.IsActive;

            var updated = await _repo.UpdateAsync(existing, cancellationToken);
            return Result<bool>.Success(updated);
        }
    }
}
