// ============================================================
// FILE: src/InventoryManagement.Domain/Models/User.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Models
{
    /// <summary>
    /// Represents a system user for authentication and authorization.
    /// </summary>
    public class User
    {
        /// <summary>Unique identifier for the user.</summary>
        [Key]
        [MaxLength(36)]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Unique username for login.</summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>User's email address.</summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>BCrypt hashed password.</summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>The user's assigned role.</summary>
        public UserRole Role { get; set; } = UserRole.Viewer;

        /// <summary>Current refresh token (null if not logged in).</summary>
        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        /// <summary>When the current refresh token expires.</summary>
        public DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>Department the user belongs to (custom claim).</summary>
        [MaxLength(50)]
        public string? Department { get; set; }

        /// <summary>Experience level: "junior" or "senior" (custom claim).</summary>
        [MaxLength(20)]
        public string? Experience { get; set; }

        /// <summary>Comma-separated warehouse IDs the user can access (custom claim).</summary>
        [MaxLength(500)]
        public string? WarehouseAccess { get; set; }
    }
}
