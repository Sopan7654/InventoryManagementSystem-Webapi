// Models/Warehouse.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Warehouse
    {
        [Key]
        [MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Capacity { get; set; }
    }
}
