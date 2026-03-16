// Features/Suppliers/Queries/GetAllSuppliers/GetAllSuppliersQuery.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
namespace InventoryManagementSystem.Features.Suppliers.Queries.GetAllSuppliers
{
    public sealed record GetAllSuppliersQuery : IRequest<Result<List<Supplier>>>;
}
