// Features/Categories/Commands/CreateCategory/CreateCategoryCommand.cs
using MediatR; using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Categories.Commands.CreateCategory
{ public sealed record CreateCategoryCommand(string CategoryName, string? Description, string? ParentCategoryId) : IRequest<Result<string>>; }
