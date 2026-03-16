// Features/Suppliers/Commands/CreateSupplier/CreateSupplierCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Suppliers.Commands.CreateSupplier
{
    public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(x => x.SupplierName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null);
            RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        }
    }
}
