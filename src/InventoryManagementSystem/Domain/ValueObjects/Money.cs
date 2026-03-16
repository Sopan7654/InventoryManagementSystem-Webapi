// Domain/ValueObjects/Money.cs
namespace InventoryManagementSystem.Domain.ValueObjects
{
    /// <summary>
    /// DDD Value Object — immutable, equality by value not identity.
    /// Represents a monetary amount with optional currency.
    /// </summary>
    public sealed record Money(decimal Amount, string Currency = "INR")
    {
        public static Money Zero => new(0m);

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException(
                    $"Cannot add amounts with different currencies: {Currency} and {other.Currency}");
            return this with { Amount = Amount + other.Amount };
        }

        public Money Multiply(decimal factor) => this with { Amount = Amount * factor };

        public override string ToString() => $"{Currency} {Amount:N2}";
    }
}
