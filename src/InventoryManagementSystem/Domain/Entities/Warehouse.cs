// Domain/Entities/Warehouse.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Warehouse
    {
        [Key][MaxLength(36)]
        public string WarehouseId { get; set; } = string.Empty;

        [Required][MaxLength(150)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Capacity { get; set; }
    }
}
