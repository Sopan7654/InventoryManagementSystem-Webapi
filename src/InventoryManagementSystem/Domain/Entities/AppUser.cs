// Domain/Entities/AppUser.cs
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    /// <summary>
    /// Application user account — stored in the AppUser table.
    /// Passwords are stored as BCrypt hashes, never plain-text.
    /// </summary>
    public class AppUser
    {
        [Key][MaxLength(36)]
        public string UserId { get; set; } = string.Empty;

        [Required][MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required][MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required][MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
