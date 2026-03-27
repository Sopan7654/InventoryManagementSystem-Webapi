// ============================================================
// FILE: src/InventoryManagement.API/Options/AppSettings.cs
// ============================================================
namespace InventoryManagement.API.Options
{
    /// <summary>Options for JWT authentication settings.</summary>
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string SecretKey { get; set; } = "DefaultSuperSecretKeyThatIsAtLeast32CharactersLong!";
        public string Issuer { get; set; } = "InventoryManagement";
        public string Audience { get; set; } = "InventoryManagementAPI";
        public int AccessTokenExpiryMinutes { get; set; } = 15;
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }

    /// <summary>Options for Redis connection.</summary>
    public class RedisSettings
    {
        public const string SectionName = "RedisSettings";
        public string ConnectionString { get; set; } = "localhost:6379";
        public int DefaultExpiryMinutes { get; set; } = 10;
    }

    /// <summary>Options for CORS configuration.</summary>
    public class CorsSettings
    {
        public const string SectionName = "CorsSettings";
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    }

    /// <summary>General application settings.</summary>
    public class AppSettings
    {
        public const string SectionName = "AppSettings";
        public string ValuationMethod { get; set; } = "FIFO";
        public int DefaultPageSize { get; set; } = 20;
        public int MaxPageSize { get; set; } = 100;
    }
}
