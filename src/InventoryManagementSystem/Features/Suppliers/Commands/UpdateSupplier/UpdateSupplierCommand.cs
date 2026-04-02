// Features/Suppliers/Commands/UpdateSupplier/UpdateSupplierCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Suppliers.Commands.UpdateSupplier
{
    public sealed record UpdateSupplierCommand(string SupplierName, string? Email, string? Phone, string? Website, string SupplierId = "")
        : IRequest<Result<bool>>;
}
