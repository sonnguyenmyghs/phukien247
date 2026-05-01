using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhuKien247.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue)]
    public decimal? SalePrice { get; set; }

    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
