// Features/Categories/Commands/UpdateCategory/UpdateCategoryCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Categories.Repository;

namespace InventoryManagementSystem.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandHandler
        : IRequestHandler<UpdateCategoryCommand, Result<string>>
    {
        private readonly ICategoryRepository _categoryRepo;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepo)
            => _categoryRepo = categoryRepo;

        public async Task<Result<string>> Handle(UpdateCategoryCommand cmd, CancellationToken ct)
        {
            var category = await _categoryRepo.GetByIdAsync(cmd.CategoryId, ct)
                ?? throw new NotFoundException($"Category '{cmd.CategoryId}' not found.");

            category.CategoryName    = cmd.CategoryName;
            category.Description     = cmd.Description;
            category.ParentCategoryId = cmd.ParentCategoryId;

            var updated = await _categoryRepo.UpdateAsync(category, ct);

            return updated
                ? Result<string>.Success($"Category '{category.CategoryName}' updated successfully.")
                : Result<string>.Failure("Failed to update category.");
        }
    }
}
