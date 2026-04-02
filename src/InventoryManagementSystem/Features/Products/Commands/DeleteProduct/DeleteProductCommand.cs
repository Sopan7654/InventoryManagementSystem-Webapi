// Features/Products/Commands/DeleteProduct/DeleteProductCommand.cs
using InventoryManagementSystem.Common.Models;
using MediatR;

namespace InventoryManagementSystem.Features.Products.Commands.DeleteProduct
{
    /// <summary>
    /// Soft-delete a product by setting IsActive = false.
    /// Hard-delete is intentionally not supported — transaction history references
    /// must be preserved for audit trail integrity (ACID compliance).
    /// </summary>
    public record DeleteProductCommand(string ProductId) : IRequest<Result<string>>;
}
