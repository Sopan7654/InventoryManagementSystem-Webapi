// Features/Products/Commands/CreateProduct/CreateProductCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Products.Commands.CreateProduct
{
    /// <summary>CQRS Command — creates a product. Returns the new ProductId.</summary>
    public sealed record CreateProductCommand(
        string SKU,
        string ProductName,
        string? Description,
        string? CategoryId,
        string UnitOfMeasure,
        decimal Cost,
        decimal ListPrice,
        bool IsActive = true
    ) : IRequest<Result<string>>;
}
