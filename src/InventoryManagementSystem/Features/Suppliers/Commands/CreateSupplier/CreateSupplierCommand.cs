// Features/Suppliers/Commands/CreateSupplier/CreateSupplierCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
namespace InventoryManagementSystem.Features.Suppliers.Commands.CreateSupplier
{
    public sealed record CreateSupplierCommand(string SupplierName, string? Email, string? Phone, string? Website)
        : IRequest<Result<string>>;
}
