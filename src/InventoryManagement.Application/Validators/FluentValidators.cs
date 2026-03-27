// ============================================================
// FILE: src/InventoryManagement.Application/Validators/FluentValidators.cs
// ============================================================
using FluentValidation;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Validators
{
    /// <summary>Validates CreateProductDto.</summary>
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .Matches(@"^[A-Z0-9\-]{3,20}$").WithMessage("SKU must contain only uppercase letters, digits, and hyphens (3-20 chars).");

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(150);

            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0).WithMessage("Cost must be non-negative.");

            RuleFor(x => x.ListPrice)
                .GreaterThanOrEqualTo(0).WithMessage("List price must be non-negative.");
        }
    }

    /// <summary>Validates StockInDto.</summary>
    public class StockInDtoValidator : AbstractValidator<StockInDto>
    {
        public StockInDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("Warehouse ID is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }

    /// <summary>Validates StockOutDto.</summary>
    public class StockOutDtoValidator : AbstractValidator<StockOutDto>
    {
        public StockOutDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("Warehouse ID is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }

    /// <summary>Validates StockTransferDto — ensures source ≠ destination warehouse.</summary>
    public class StockTransferDtoValidator : AbstractValidator<StockTransferDto>
    {
        public StockTransferDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.FromWarehouseId).NotEmpty().WithMessage("Source warehouse ID is required.");
            RuleFor(x => x.ToWarehouseId).NotEmpty().WithMessage("Destination warehouse ID is required.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x)
                .Must(x => !string.Equals(x.FromWarehouseId, x.ToWarehouseId, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Source and destination warehouses must be different.");
        }
    }

    /// <summary>Validates AdjustmentDto.</summary>
    public class AdjustmentDtoValidator : AbstractValidator<AdjustmentDto>
    {
        public AdjustmentDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("Warehouse ID is required.");
            RuleFor(x => x.Quantity).NotEqual(0).WithMessage("Adjustment quantity cannot be zero.");
        }
    }

    /// <summary>Validates CreateBatchDto with FutureDate on ExpiryDate.</summary>
    public class CreateBatchDtoValidator : AbstractValidator<CreateBatchDto>
    {
        public CreateBatchDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.BatchNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.ExpiryDate.HasValue)
                .WithMessage("Expiry date must be in the future.");
        }
    }

    /// <summary>Validates CreatePurchaseOrderDto.</summary>
    public class CreatePurchaseOrderDtoValidator : AbstractValidator<CreatePurchaseOrderDto>
    {
        public CreatePurchaseOrderDtoValidator()
        {
            RuleFor(x => x.SupplierId).NotEmpty().WithMessage("Supplier ID is required.");
            RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required.");
            RuleForEach(x => x.Items).SetValidator(new AddPurchaseOrderItemDtoValidator());
        }
    }

    /// <summary>Validates AddPurchaseOrderItemDto.</summary>
    public class AddPurchaseOrderItemDtoValidator : AbstractValidator<AddPurchaseOrderItemDto>
    {
        public AddPurchaseOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.QuantityOrdered).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThan(0);
        }
    }

    /// <summary>Validates RegisterDto.</summary>
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }

    /// <summary>Validates LoginDto.</summary>
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
