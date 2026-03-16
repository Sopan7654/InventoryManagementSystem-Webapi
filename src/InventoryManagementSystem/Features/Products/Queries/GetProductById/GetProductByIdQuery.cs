// Features/Products/Queries/GetProductById/GetProductByIdQuery.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Products.Queries.GetProductById
{
    /// <summary>CQRS Query — fetches a single product by its ID.</summary>
    public sealed record GetProductByIdQuery(string ProductId) : IRequest<Result<Product>>;
}
