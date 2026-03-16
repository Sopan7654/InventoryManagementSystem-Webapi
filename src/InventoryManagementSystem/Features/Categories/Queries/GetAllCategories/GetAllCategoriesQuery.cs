// Features/Categories/Queries/GetAllCategories/GetAllCategoriesQuery.cs
using MediatR; using InventoryManagementSystem.Common.Models; using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Categories.Queries.GetAllCategories
{ public sealed record GetAllCategoriesQuery : IRequest<Result<List<ProductCategory>>>; }
