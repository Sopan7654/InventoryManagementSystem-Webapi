// Features/Products/Commands/UpdateProduct/UpdateProductCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Products.Commands.UpdateProduct
{
    public sealed record UpdateProductCommand(
        string ProductId,
        string SKU,
        string ProductName,
        string? Description,
        string? CategoryId,
        string UnitOfMeasure,
        decimal Cost,
        decimal ListPrice,
        bool IsActive
    ) : IRequest<Result<bool>>;
}
