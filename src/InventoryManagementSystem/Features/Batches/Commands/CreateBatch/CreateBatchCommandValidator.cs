// Features/Batches/Commands/CreateBatch/CreateBatchCommandValidator.cs
using FluentValidation;
namespace InventoryManagementSystem.Features.Batches.Commands.CreateBatch
{
    public sealed class CreateBatchCommandValidator : AbstractValidator<CreateBatchCommand>
    {
        public CreateBatchCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.BatchNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Batch quantity must be greater than zero.");
            RuleFor(x => x.ExpiryDate).GreaterThan(x => x.ManufacturingDate)
                .When(x => x.ExpiryDate.HasValue && x.ManufacturingDate.HasValue)
                .WithMessage("Expiry date must be after manufacturing date.");
        }
    }
}
