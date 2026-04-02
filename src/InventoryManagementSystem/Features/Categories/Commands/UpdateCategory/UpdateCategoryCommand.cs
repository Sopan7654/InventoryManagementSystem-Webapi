// Features/Categories/Commands/UpdateCategory/UpdateCategoryCommand.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(
        string CategoryName,
        string? Description,
        string? ParentCategoryId,
        string CategoryId = ""
    ) : IRequest<Result<string>>;
}
