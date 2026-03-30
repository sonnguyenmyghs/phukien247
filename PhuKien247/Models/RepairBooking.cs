using System.ComponentModel.DataAnnotations;

namespace PhuKien247.Models;

public enum RepairStatus
{
    ChoXacNhan,    // Chờ xác nhận
    DaTiepNhan,    // Đã tiếp nhận
    DangSua,       // Đang sửa
    HoanThanh,     // Hoàn thành
    DaHuy          // Đã hủy
}

public class RepairBooking
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ServiceId { get; set; }
    public RepairService Service { get; set; } = null!;

    [Required, MaxLength(200)]
    public string DeviceInfo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? IssueDescription { get; set; }

    public DateTime BookingDate { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.ChoXacNhan;

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
