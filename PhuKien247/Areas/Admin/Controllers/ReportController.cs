using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhuKien247.Data;
using PhuKien247.Models;

namespace PhuKien247.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    private (DateTime from, DateTime to) NormalizeDates(DateTime? fromDate, DateTime? toDate)
    {
        var to = (toDate ?? DateTime.Now).Date.AddDays(1).AddSeconds(-1);
        var from = (fromDate ?? DateTime.Now.AddDays(-30)).Date;
        if (from > to) (from, to) = (to.Date, from.Date.AddDays(1).AddSeconds(-1));
        return (from, to);
    }

    // 1. Doanh thu theo ngày
    public async Task<IActionResult> Revenue(DateTime? fromDate, DateTime? toDate)
    {
        var (from, to) = NormalizeDates(fromDate, toDate);

        var orders = await _context.Orders
            .Where(o => o.Status == OrderStatus.HoanThanh && o.CreatedAt >= from && o.CreatedAt <= to)
            .ToListAsync();

        var grouped = orders
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount), Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList();

        // Fill missing days
        var allDays = new List<DateTime>();
        for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
            allDays.Add(d);

        var labels = allDays.Select(d => d.ToString("dd/MM")).ToList();
        var data = allDays.Select(d => grouped.FirstOrDefault(x => x.Date == d)?.Total ?? 0m).ToList();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.Labels = labels;
        ViewBag.Data = data;
        ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
        ViewBag.TotalOrders = orders.Count;
        ViewBag.AverageOrder = orders.Count > 0 ? orders.Sum(o => o.TotalAmount) / orders.Count : 0m;
        ViewBag.Details = grouped;

        return View();
    }

    // 2. Top sản phẩm bán chạy
    public async Task<IActionResult> TopProducts(DateTime? fromDate, DateTime? toDate)
    {
        var (from, to) = NormalizeDates(fromDate, toDate);

        var rawData = await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .Where(od => od.Order.Status == OrderStatus.HoanThanh
                      && od.Order.CreatedAt >= from && od.Order.CreatedAt <= to)
            .ToListAsync();

        var data = rawData
            .GroupBy(od => new { od.ProductId, od.Product.Name, od.Product.ImageUrl, od.Product.Price })
            .Select(g => new TopProductItem
            {
                ProductId = g.Key.ProductId,
                Name = g.Key.Name,
                ImageUrl = g.Key.ImageUrl,
                Price = g.Key.Price,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(10)
            .ToList();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.Labels = data.Select(d => d.Name).ToList();
        ViewBag.Quantities = data.Select(d => d.TotalQuantity).ToList();
        ViewBag.TotalQuantitySold = data.Sum(d => d.TotalQuantity);
        ViewBag.TotalProductRevenue = data.Sum(d => d.TotalRevenue);

        return View(data);
    }

    // 3. Top khách hàng VIP
    public async Task<IActionResult> TopCustomers(DateTime? fromDate, DateTime? toDate)
    {
        var (from, to) = NormalizeDates(fromDate, toDate);

        var rawData = await _context.Orders
            .Include(o => o.User)
            .Where(o => o.Status == OrderStatus.HoanThanh && o.CreatedAt >= from && o.CreatedAt <= to)
            .ToListAsync();

        var data = rawData
            .GroupBy(o => new { o.UserId, o.User.FullName, o.User.Email, o.User.PhoneNumber })
            .Select(g => new TopCustomerItem
            {
                UserId = g.Key.UserId,
                FullName = g.Key.FullName,
                Email = g.Key.Email ?? string.Empty,
                PhoneNumber = g.Key.PhoneNumber ?? string.Empty,
                TotalSpent = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(10)
            .ToList();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.Labels = data.Select(d => d.FullName).ToList();
        ViewBag.Spents = data.Select(d => d.TotalSpent).ToList();
        ViewBag.TotalCustomers = data.Count;
        ViewBag.TotalAllSpent = data.Sum(d => d.TotalSpent);

        return View(data);
    }

    // 4. Doanh thu theo danh mục
    public async Task<IActionResult> RevenueByCategory(DateTime? fromDate, DateTime? toDate)
    {
        var (from, to) = NormalizeDates(fromDate, toDate);

        var rawData = await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product).ThenInclude(p => p.Category)
            .Where(od => od.Order.Status == OrderStatus.HoanThanh
                      && od.Order.CreatedAt >= from && od.Order.CreatedAt <= to)
            .ToListAsync();

        var data = rawData
            .GroupBy(od => new { od.Product.CategoryId, od.Product.Category.Name })
            .Select(g => new RevenueByCategoryItem
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.Labels = data.Select(d => d.CategoryName).ToList();
        ViewBag.Revenues = data.Select(d => d.TotalRevenue).ToList();
        ViewBag.TotalRevenue = data.Sum(d => d.TotalRevenue);
        ViewBag.TotalCategories = data.Count;

        return View(data);
    }

    // 5. Tồn kho thấp
    public async Task<IActionResult> LowStock(DateTime? fromDate, DateTime? toDate)
    {
        // fromDate/toDate là chuẩn signature, nhưng LowStock không lọc theo ngày
        var (from, to) = NormalizeDates(fromDate, toDate);

        var data = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.Stock < 10)
            .OrderBy(p => p.Stock)
            .ToListAsync();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.TotalLow = data.Count;
        ViewBag.TotalOut = data.Count(p => p.Stock == 0);
        ViewBag.TotalUnder5 = data.Count(p => p.Stock > 0 && p.Stock < 5);

        return View(data);
    }

    // 6. Top dịch vụ sửa chữa
    public async Task<IActionResult> TopServices(DateTime? fromDate, DateTime? toDate)
    {
        var (from, to) = NormalizeDates(fromDate, toDate);

        var rawData = await _context.RepairBookings
            .Include(r => r.Service)
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
            .ToListAsync();

        var data = rawData
            .GroupBy(r => new { r.ServiceId, r.Service.Name, r.Service.Price })
            .Select(g => new TopServiceItem
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.Name,
                Price = g.Key.Price,
                BookingCount = g.Count(),
                CompletedCount = g.Count(x => x.Status == RepairStatus.HoanThanh)
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(10)
            .ToList();

        ViewBag.FromDate = from;
        ViewBag.ToDate = to;
        ViewBag.Labels = data.Select(d => d.ServiceName).ToList();
        ViewBag.Counts = data.Select(d => d.BookingCount).ToList();
        ViewBag.TotalBookings = data.Sum(d => d.BookingCount);
        ViewBag.TotalServices = data.Count;

        return View(data);
    }
}

public class TopProductItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TopCustomerItem
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int OrderCount { get; set; }
}

public class RevenueByCategoryItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class TopServiceItem
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int BookingCount { get; set; }
    public int CompletedCount { get; set; }
}
