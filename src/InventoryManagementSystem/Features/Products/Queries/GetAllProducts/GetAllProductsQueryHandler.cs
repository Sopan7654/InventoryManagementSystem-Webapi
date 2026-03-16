// Features/Products/Queries/GetAllProducts/GetAllProductsQueryHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Products.Repository;

namespace InventoryManagementSystem.Features.Products.Queries.GetAllProducts
{
    public sealed class GetAllProductsQueryHandler
        : IRequestHandler<GetAllProductsQuery, Result<List<Product>>>
    {
        private readonly IProductRepository _repo;

        public GetAllProductsQueryHandler(IProductRepository repo) => _repo = repo;

        public async Task<Result<List<Product>>> Handle(
            GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync(cancellationToken);
            return Result<List<Product>>.Success(products);
        }
    }
}
