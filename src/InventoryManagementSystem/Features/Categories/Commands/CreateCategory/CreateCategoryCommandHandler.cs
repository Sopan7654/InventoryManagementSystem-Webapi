// Features/Categories/Commands/CreateCategory/CreateCategoryCommandHandler.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities; using InventoryManagementSystem.Features.Categories.Repository;
namespace InventoryManagementSystem.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<string>>
    {
        private readonly ICategoryRepository _repo;
        public CreateCategoryCommandHandler(ICategoryRepository repo) => _repo = repo;
        public async Task<Result<string>> Handle(CreateCategoryCommand cmd, CancellationToken ct)
        {
            var cat = new ProductCategory { CategoryId = Guid.NewGuid().ToString(), CategoryName = cmd.CategoryName, Description = cmd.Description, ParentCategoryId = cmd.ParentCategoryId };
            await _repo.InsertAsync(cat, ct);
            return Result<string>.Success(cat.CategoryId);
        }
    }
}
