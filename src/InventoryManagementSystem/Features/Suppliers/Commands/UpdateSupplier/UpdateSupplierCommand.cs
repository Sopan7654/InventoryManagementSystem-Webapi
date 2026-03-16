// Features/Suppliers/Commands/UpdateSupplier/UpdateSupplierCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Suppliers.Commands.UpdateSupplier
{
    public sealed record UpdateSupplierCommand(string SupplierId, string SupplierName, string? Email, string? Phone, string? Website)
        : IRequest<Result<bool>>;
}
