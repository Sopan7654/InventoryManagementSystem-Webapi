// Features/Suppliers/Queries/GetSupplierById/GetSupplierByIdQuery.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Suppliers.Queries.GetSupplierById
{
    public sealed record GetSupplierByIdQuery(string SupplierId) : IRequest<Result<Supplier>>;
}
