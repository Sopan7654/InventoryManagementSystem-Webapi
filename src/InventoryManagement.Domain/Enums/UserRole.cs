// ============================================================
// FILE: src/InventoryManagement.Domain/Enums/UserRole.cs
// ============================================================
namespace InventoryManagement.Domain.Enums
{
    /// <summary>
    /// Defines the roles available in the system for role-based authorization.
    /// </summary>
    public enum UserRole
    {
        /// <summary>Read-only access to all data.</summary>
        Viewer,

        /// <summary>Can perform day-to-day inventory operations.</summary>
        Operator,

        /// <summary>Can manage products, approve orders, and view reports.</summary>
        Manager,

        /// <summary>Full system access including user management.</summary>
        Admin
    }
}
