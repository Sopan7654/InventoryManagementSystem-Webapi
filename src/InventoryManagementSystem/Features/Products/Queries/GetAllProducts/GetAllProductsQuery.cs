// Features/Products/Queries/GetAllProducts/GetAllProductsQuery.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Products.Queries.GetAllProducts
{
    /// <summary>CQRS Query — fetches all products. No side-effects.</summary>
    public sealed record GetAllProductsQuery : IRequest<Result<List<Product>>>;
}
