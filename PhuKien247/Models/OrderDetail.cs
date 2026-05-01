using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhuKien247.Models;

public class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,0)")]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
