// Features/Warehouses/Commands/CreateWarehouse/CreateWarehouseCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Warehouses.Commands.CreateWarehouse
{
    public sealed class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseCommandValidator()
        {
            RuleFor(x => x.WarehouseName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Capacity).GreaterThan(0).When(x => x.Capacity.HasValue);
        }
    }
}
