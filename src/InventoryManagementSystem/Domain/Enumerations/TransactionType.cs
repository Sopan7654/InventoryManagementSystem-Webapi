// Domain/Enumerations/TransactionType.cs
namespace InventoryManagementSystem.Domain.Enumerations
{
    /// <summary>
    /// Strongly-typed enumeration replacing magic strings for stock transaction types.
    /// DDD: belongs in the domain layer — no infrastructure dependency.
    /// </summary>
    public static class TransactionType
    {
        public const string Purchase    = "PURCHASE";
        public const string Sale        = "SALE";
        public const string Adjustment  = "ADJUSTMENT";
        public const string TransferIn  = "TRANSFER_IN";
        public const string TransferOut = "TRANSFER_OUT";
        public const string Return      = "RETURN";
        public const string Hold        = "HOLD";
        public const string HoldRelease = "HOLD_RELEASE";
        public const string WriteOff    = "WRITE_OFF";

        public static readonly IReadOnlyList<string> All = new[]
        {
            Purchase, Sale, Adjustment, TransferIn, TransferOut,
            Return, Hold, HoldRelease, WriteOff
        };
    }
}
