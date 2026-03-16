// Features/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler
        : IRequestHandler<GetProductByIdQuery, Result<Product>>
    {
        private readonly IProductRepository _repo;

        public GetProductByIdQueryHandler(IProductRepository repo) => _repo = repo;

        public async Task<Result<Product>> Handle(
            GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.ProductId, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), request.ProductId);

            return Result<Product>.Success(product);
        }
    }
}
