using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhuKien247.Models;

public class RepairService
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [MaxLength(100)]
    public string? EstimatedTime { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<RepairBooking> RepairBookings { get; set; } = new List<RepairBooking>();
}
