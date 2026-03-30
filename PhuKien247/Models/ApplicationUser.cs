using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PhuKien247.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
