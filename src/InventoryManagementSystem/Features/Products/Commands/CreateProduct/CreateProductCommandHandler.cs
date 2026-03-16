// Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandHandler
        : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IProductRepository _repo;

        public CreateProductCommandHandler(IProductRepository repo) => _repo = repo;

        public async Task<Result<string>> Handle(
            CreateProductCommand cmd, CancellationToken cancellationToken)
        {
            if (await _repo.SKUExistsAsync(cmd.SKU, ct: cancellationToken))
                throw new DomainException($"SKU '{cmd.SKU}' already exists.");

            var product = new Product
            {
                ProductId     = Guid.NewGuid().ToString(),
                SKU           = cmd.SKU,
                ProductName   = cmd.ProductName,
                Description   = cmd.Description,
                CategoryId    = cmd.CategoryId,
                UnitOfMeasure = cmd.UnitOfMeasure,
                Cost          = cmd.Cost,
                ListPrice     = cmd.ListPrice,
                IsActive      = cmd.IsActive
            };

            await _repo.InsertAsync(product, cancellationToken);
            return Result<string>.Success(product.ProductId);
        }
    }
}
