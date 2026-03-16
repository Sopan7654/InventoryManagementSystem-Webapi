// Features/Categories/Queries/GetAllCategories/GetAllCategoriesQueryHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Categories.Repository;
namespace InventoryManagementSystem.Features.Categories.Queries.GetAllCategories
{
    public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<ProductCategory>>>
    {
        private readonly ICategoryRepository _repo;
        public GetAllCategoriesQueryHandler(ICategoryRepository repo) => _repo = repo;
        public async Task<Result<List<ProductCategory>>> Handle(GetAllCategoriesQuery req, CancellationToken ct)
            => Result<List<ProductCategory>>.Success(await _repo.GetAllAsync(ct));
    }
}
