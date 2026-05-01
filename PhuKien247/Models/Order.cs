using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhuKien247.Models;

public enum OrderStatus
{
    ChoXacNhan,    // Chờ xác nhận
    DaXacNhan,     // Đã xác nhận
    DangGiao,      // Đang giao
    HoanThanh,     // Hoàn thành
    DaHuy          // Đã hủy
}

public enum PaymentMethod
{
    COD,           // Thanh toán khi nhận hàng
    BankTransfer   // Chuyển khoản
}

public class Order
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.ChoXacNhan;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidAt { get; set; }

    [Required, MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
