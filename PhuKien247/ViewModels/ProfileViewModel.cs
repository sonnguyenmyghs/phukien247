using System.ComponentModel.DataAnnotations;

namespace PhuKien247.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [MaxLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }
}
