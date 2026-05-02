using System.ComponentModel.DataAnnotations;
using PhuKien247.Models;
namespace PhuKien247.ViewModels;
public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0901234567)")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Note { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

    public List<CartItem> CartItems { get; set; } = new();
    public decimal TotalAmount => CartItems.Sum(x => x.Total);
}
