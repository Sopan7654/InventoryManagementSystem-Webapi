// Features/Categories/Commands/CreateCategory/CreateCategoryCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(100);
        }
    }
}
