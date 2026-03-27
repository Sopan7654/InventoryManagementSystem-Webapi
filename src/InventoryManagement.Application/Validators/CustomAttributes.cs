// ============================================================
// FILE: src/InventoryManagement.Application/Validators/CustomAttributes.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Application.Validators
{
    /// <summary>
    /// Validates that a SKU matches the pattern: uppercase letters, digits, and hyphens (3-20 chars).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ValidSKUAttribute : ValidationAttribute
    {
        private const string Pattern = @"^[A-Z0-9\-]{3,20}$";

        public ValidSKUAttribute() : base("SKU must contain only uppercase letters, digits, and hyphens (3-20 characters).")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is not string sku) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(sku, Pattern);
        }
    }

    /// <summary>
    /// Validates that a date is in the future.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute() : base("Date must be in the future.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is null) return true; // Allow null; use [Required] separately.
            if (value is DateTime date) return date > DateTime.Today;
            return false;
        }
    }
}
